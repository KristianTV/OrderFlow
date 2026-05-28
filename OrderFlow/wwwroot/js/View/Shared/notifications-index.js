function initializeNotifications() {
    const list = document.getElementById("notificationsList");
    const hideSystem = document.getElementById("hideSystemNotifications");
    const empty = document.getElementById("notificationsEmpty");
    const sortButton = document.getElementById("sortDropdown");
    const sortLinks = Array.from(document.querySelectorAll("[data-notification-sort]"));
    const form = document.getElementById("notificationFilterForm");

    if (!list) {
        return;
    }

    // Call setupEndlessIndex
    setupEndlessIndex({
        formId: "notificationFilterForm",
        listId: "notificationsList",
        sentinelId: "notificationsSentinel",
        emptyId: "notificationsEmpty"
    });

    // Update active state in dropdown and the dropdown label when a sort option is selected
    sortLinks.forEach(link => {
        link.addEventListener("click", event => {
            const sortBy = link.dataset.notificationSort;
            if (sortButton) {
                const label = sortBy.charAt(0).toUpperCase() + sortBy.slice(1);
                sortButton.innerHTML = `<i class="fas fa-sort me-2"></i> Sort By: ${label}`;
            }
            sortLinks.forEach(l => {
                l.classList.toggle("active", l.dataset.notificationSort === sortBy);
            });
        });
    });
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initializeNotifications);
} else {
    initializeNotifications();
}
