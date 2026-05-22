function fillCourseOptions(select, courses, placeholder, selectedValue, disableWhenEmpty) {
    select.innerHTML = "";

    const placeholderOption = document.createElement("option");
    placeholderOption.value = "";
    placeholderOption.textContent = placeholder;
    select.appendChild(placeholderOption);

    courses.forEach(course => {
        const option = document.createElement("option");
        option.value = course.id;
        option.textContent = course.text;
        option.selected = selectedValue && selectedValue.toLowerCase() === course.id.toLowerCase();
        select.appendChild(option);
    });

    select.disabled = disableWhenEmpty && !selectedValue && courses.length === 0;
}

async function loadTruckCourses(select, url, truckId, placeholder, selectedValue, disableWhenEmpty) {
    const response = await fetch(`${url}?truckId=${encodeURIComponent(truckId || "")}`, {
        headers: { "X-Requested-With": "XMLHttpRequest" }
    });

    if (!response.ok) {
        return;
    }

    const courses = await response.json();
    if (courses.length === 0) {
        placeholder = "No courses available";
        disableWhenEmpty = true;
    }
    fillCourseOptions(select, courses, placeholder, selectedValue, disableWhenEmpty);
}

function setupTruckCourseFilter(options) {
    const truckSelect = document.getElementById(options.truckSelectId);
    const courseSelect = document.getElementById(options.courseSelectId);

    if (!truckSelect || !courseSelect) {
        return;
    }

    truckSelect.addEventListener("change", async () => {
        if (options.disableWithoutTruck && !truckSelect.value) {
            fillCourseOptions(courseSelect, [], "Select truck first", "", true);
            courseSelect.disabled = true;
            return;
        }

        await loadTruckCourses(
            courseSelect,
            options.coursesUrl,
            truckSelect.value,
            options.placeholder,
            "",
            options.disableWithoutTruck);
    });

    if (options.disableWithoutTruck) {
        courseSelect.disabled = !truckSelect.value;
    }
}

function setupTruckSpendingIndex(options) {
    const form = document.getElementById(options.formId);
    const results = document.getElementById(options.resultsId);

    if (!form || !results) {
        return;
    }

    setupTruckCourseFilter(options);

    form.addEventListener("submit", async event => {
        event.preventDefault();

        const url = `${form.action}?${new URLSearchParams(new FormData(form)).toString()}`;
        const response = await fetch(url, {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });

        if (!response.ok) {
            return;
        }

        results.innerHTML = await response.text();
        window.history.replaceState(null, "", url);
    });
}
