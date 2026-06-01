(function () {
    if (!window.signalR) {
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/app")
        .withAutomaticReconnect()
        .build();

    let isPageNavigating = false;
    let navigationTimer;

    connection.on("NotificationCountChanged", updateNotificationBadge);
    connection.on("EntityChanged", handleEntityChanged);

    document.addEventListener("submit", () => {
        isPageNavigating = true;
        clearTimeout(navigationTimer);
        navigationTimer = setTimeout(() => {
            isPageNavigating = false;
        }, 15000);
    }, true);

    window.addEventListener("beforeunload", () => {
        isPageNavigating = true;
    });

    connection.start().catch(err => console.error("SignalR app connection error:", err));

    function updateNotificationBadge(payload) {
        document.querySelectorAll("[data-notification-badge]").forEach(badge => {
            const count = Number(payload?.count ?? 0);
            badge.textContent = payload?.display ?? (count > 99 ? "99+" : count.toString());
            badge.classList.toggle("d-none", count <= 0);
        });
    }

    function handleEntityChanged(change) {
        if (isPageNavigating || !change || !shouldRefresh(change)) {
            return;
        }

        window.location.reload();
    }

    function shouldRefresh(change) {
        const pathParts = window.location.pathname.split("/").filter(Boolean);
        const area = ["Admin", "Driver"].includes(pathParts[0]) ? pathParts[0] : "";
        const controller = area ? pathParts[1] : pathParts[0];
        const action = area ? pathParts[2] : pathParts[1];
        const routeId = (area ? pathParts[3] : pathParts[2])?.toLowerCase();
        const entity = String(change.entity || "");
        const id = String(change.id || "").toLowerCase();
        const relatedId = String(change.relatedId || "").toLowerCase();

        if (!controller) {
            return false;
        }

        if (controller === "Dashboard") {
            return ["Order", "Payment", "Course", "Truck", "TruckSpending", "Notification", "Account"].includes(entity);
        }

        if (controller === "Order") {
            if (entity === "Payment") {
                return !routeId || routeId === relatedId;
            }

            return entity === "Order" && (!routeId || routeId === id);
        }

        if (controller === "Course") {
            return entity === "Course" && (!routeId || routeId === id);
        }

        if (controller === "Truck") {
            return entity === "Truck" && (!routeId || routeId === id);
        }

        if (controller === "TruckSpending") {
            return entity === "TruckSpending" && (!routeId || routeId === id);
        }

        if (controller === "Notification") {
            return entity === "Notification" && (!routeId || routeId === id);
        }

        if (controller === "Account") {
            return entity === "Account" && (!routeId || routeId === id);
        }

        if (controller === "Payment") {
            return entity === "Payment" && (!routeId || routeId === id);
        }

        return false;
    }
})();
