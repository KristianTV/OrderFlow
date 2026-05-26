function setupEndlessIndex(options) {
    const form = options.formId ? document.getElementById(options.formId) : null;
    const list = document.getElementById(options.listId);
    const sentinel = document.getElementById(options.sentinelId);
    const empty = options.emptyId ? document.getElementById(options.emptyId) : null;

    if (!list || !sentinel) {
        return;
    }

    let page = Number(sentinel.dataset.page || "1");
    let hasMore = sentinel.dataset.hasMore === "true";
    let loading = false;
    let resetTimer = null;

    let lastSubmitter = null;

    function getFormParams() {
        const params = form ? new URLSearchParams(new FormData(form)) : new URLSearchParams(window.location.search);

        if (lastSubmitter && lastSubmitter.name) {
            params.set(lastSubmitter.name, lastSubmitter.value);
        }

        for (const [key, value] of Array.from(params.entries())) {
            if (value === null || value.trim() === "") {
                params.delete(key);
            }
        }

        return params;
    }

    function buildUrl(nextPage) {
        const baseUrl = form ? form.action : window.location.pathname;
        const params = getFormParams();

        if (nextPage > 1) {
            params.set("page", nextPage.toString());
        } else {
            params.delete("page");
        }

        const query = params.toString();
        return query ? `${baseUrl}?${query}` : baseUrl;
    }

    async function load(nextPage, replace) {
        if (loading || (!hasMore && !replace)) {
            return;
        }

        loading = true;
        sentinel.classList.remove("d-none");

        const response = await fetch(buildUrl(nextPage), {
            headers: { "X-Requested-With": "XMLHttpRequest" }
        });

        if (response.ok) {
            const html = await response.text();
            if (replace) {
                list.innerHTML = html;
            } else {
                list.insertAdjacentHTML("beforeend", html);
            }

            page = nextPage;
            hasMore = response.headers.get("X-Has-More") === "true";
            sentinel.dataset.page = page.toString();
            sentinel.dataset.hasMore = hasMore.toString();
            sentinel.classList.toggle("d-none", !hasMore);

            if (empty) {
                empty.classList.toggle("d-none", list.children.length > 0);
            }

            window.history.replaceState(null, "", buildUrl(1));
        }

        loading = false;
    }

    function resetSoon() {
        window.clearTimeout(resetTimer);
        resetTimer = window.setTimeout(() => load(1, true), options.inputDelay || 250);
    }

    if (form) {
        form.addEventListener("submit", event => {
            event.preventDefault();
            lastSubmitter = event.submitter || null;
            load(1, true);
        });

        form.querySelectorAll("select,input[type='checkbox']").forEach(element => {
            element.addEventListener("change", () => {
                lastSubmitter = null;
                load(1, true);
            });
        });

        form.querySelectorAll("input[type='text'],input[type='search']").forEach(element => {
            element.addEventListener("input", () => {
                lastSubmitter = null;
                resetSoon();
            });
        });
    }

    document.querySelectorAll("[data-endless-link]").forEach(link => {
        link.addEventListener("click", event => {
            event.preventDefault();
            const url = new URL(link.href);
            if (form) {
                for (const [key, value] of url.searchParams.entries()) {
                    const input = form.elements[key];
                    if (input) {
                        input.value = value;
                    }
                }
            }
            window.history.replaceState(null, "", link.href);
            load(1, true);
        });
    });

    new IntersectionObserver(entries => {
        if (entries.some(entry => entry.isIntersecting)) {
            load(page + 1, false);
        }
    }, { rootMargin: "300px" }).observe(sentinel);
}
