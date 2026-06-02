(function () {
    function byId(id) {
        return document.getElementById(id);
    }

    function createOption(value, text, selected) {
        const option = document.createElement("option");
        option.value = value;
        option.textContent = text;
        option.selected = selected;
        return option;
    }

    function appendSeparator(select) {
        const separator = document.createElement("option");
        separator.disabled = true;
        separator.textContent = "──────────";
        select.appendChild(separator);
    }

    function appendGroup(select, items, selectedValue) {
        items.forEach(item => {
            const itemId = String(item.id);
            select.appendChild(createOption(itemId, item.text, itemId.toLowerCase() === selectedValue.toLowerCase()));
        });
    }

    function hasSelectableItems(data) {
        const suggested = data && Array.isArray(data.suggested) ? data.suggested : [];
        const all = data && Array.isArray(data.all) ? data.all : [];

        return suggested.length > 0 || all.length > 0;
    }

    function populateSelect(select, data, placeholder, emptyText) {
        const selectedValue = select.value;
        select.innerHTML = "";

        const suggested = data && Array.isArray(data.suggested) ? data.suggested : [];
        const all = data && Array.isArray(data.all) ? data.all : [];
        const hasItems = suggested.length > 0 || all.length > 0;

        if (!hasItems) {
            select.appendChild(createOption("", emptyText, true));
            return;
        }
        select.appendChild(createOption("", placeholder, !selectedValue));

        appendGroup(select, suggested, selectedValue);

        if (suggested.length > 0 && all.length > 0) {
            appendSeparator(select);
        }

        appendGroup(select, all, selectedValue);

        if (selectedValue && select.value !== selectedValue) {
            select.value = "";
        }
    }
    function setSelectAvailability(select, isAvailable) {
        select.disabled = !isAvailable;

        if (!isAvailable) {
            select.value = "";
        }
    }

    function clearSelects(selects) {
        selects.forEach(select => {
            if (select) {
                select.value = "";
            }
        });
    }

    async function refreshNotificationOptions(config) {
        const receiverSelect = byId("ReceiverId");
        const orderSelect = byId("OrderId");
        const paymentSelect = byId("PaymentId");
        const truckSelect = byId("TruckId");
        const courseSelect = byId("CourseId");
        const truckSpendingSelect = byId("TruckSpendingId");

        if (!receiverSelect || !orderSelect || !paymentSelect || !truckSelect || !courseSelect || !truckSpendingSelect) {
            return;
        }

        const url = new URL(config.optionsUrl, window.location.origin);
        if (receiverSelect.value) {
            url.searchParams.set("receiverId", receiverSelect.value);
        }
        if (orderSelect.value) {
            url.searchParams.set("orderId", orderSelect.value);
        }
        if (truckSelect.value) {
            url.searchParams.set("truckId", truckSelect.value);
        }
        if (courseSelect.value) {
            url.searchParams.set("courseId", courseSelect.value);
        }

        const response = await fetch(url.toString(), {
            headers: {
                "Accept": "application/json"
            }
        });

        if (!response.ok) {
            return;
        }

        const options = await response.json();

        populateSelect(orderSelect, options.orders, "-- Select Order --", "-- No Orders Available --");
        populateSelect(paymentSelect, options.payments, "-- Select Payment --", "-- No Payment Available --");
        populateSelect(truckSelect, options.trucks, "-- Select Truck --", "-- No Trucks Available --");
        populateSelect(courseSelect, options.courses, "-- Select Course --", "-- No Courses Available --");
        populateSelect(truckSpendingSelect, options.truckSpendings, "-- Select Truck Spending --", "-- No Truck Spendings Available --");

        const hasReceiver = Boolean(receiverSelect.value);
        setSelectAvailability(orderSelect, hasReceiver && hasSelectableItems(options.orders));
        setSelectAvailability(truckSelect, hasReceiver && hasSelectableItems(options.trucks));

        const hasOrder = Boolean(orderSelect.value);
        const hasTruck = Boolean(truckSelect.value);

        setSelectAvailability(paymentSelect, hasOrder && hasSelectableItems(options.payments));
        setSelectAvailability(courseSelect, hasTruck && hasSelectableItems(options.courses));
        setSelectAvailability(truckSpendingSelect, hasTruck && hasSelectableItems(options.truckSpendings));
    }

    window.initAdminNotificationForm = function (config) {
        const receiverSelect = byId("ReceiverId");
        const orderSelect = byId("OrderId");
        const truckSelect = byId("TruckId");
        const courseSelect = byId("CourseId");

        if (!receiverSelect || !orderSelect || !truckSelect || !courseSelect || !config || !config.optionsUrl) {
            return Promise.resolve();
        }

        const paymentSelect = byId("PaymentId");
        const truckSpendingSelect = byId("TruckSpendingId");

        const refresh = () => refreshNotificationOptions(config);

        receiverSelect.addEventListener("change", function () {
            clearSelects([orderSelect, paymentSelect, truckSelect, courseSelect, truckSpendingSelect]);
            refresh();
        });

        orderSelect.addEventListener("change", function () {
            clearSelects([paymentSelect]);
            refresh();
        });

        truckSelect.addEventListener("change", function () {
            clearSelects([courseSelect, truckSpendingSelect]);
            refresh();
        });

        courseSelect.addEventListener("change", function () {
            clearSelects([truckSpendingSelect]);
            refresh();
        });

        return refresh();
    };
})();
