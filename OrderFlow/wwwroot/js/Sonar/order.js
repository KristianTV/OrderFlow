var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/order")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveOrderUpdate", (indexOrder) => {
    var noOrdersMessage = document.getElementById("noOrdersMessage");
    if (noOrdersMessage) {
        noOrdersMessage.remove();
    }

    var orderCard = `
         <div class="col">
                    <div class="card h-100 shadow-sm border-primary">
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title text-primary mb-2"><i class="fas fa-box-open me-2"></i> Order ID: ${indexOrder.orderID}</h5>
                            <h6 class="card-subtitle mb-3 text-muted">
                                <i class="far fa-calendar-alt me-1"></i> Date: ${indexOrder.orderDate.ToString("yyyy-MM-dd HH:mm")}
                            </h6>
                            <p class="card-text mb-1">
                                <strong><i class="fas fa-truck-loading me-1"></i> Delivery:</strong>  ${indexOrder.deliveryAddress}
                            </p>
                            <p class="card-text mb-3">
                                <strong><i class="fas fa-map-marker-alt me-1"></i> Pickup:</strong>  ${indexOrder.pickupAddress}
                            </p>
                            <p class="card-text mt-auto">
                                <strong>Status:</strong>
                                <span class="badge ${GetStatusBadgeClass(order.Status)} fs-6">
                                    ${indexOrder.status}
                                </span>
                            </p>
                        </div>
                        <div class="card-footer text-end bg-light border-top-0">
                             <a class="btn btn-primary" href="/Admin/Order/Detail/${indexOrder.orderID}">
                                <i class="fas fa-info-circle me-2"></i> View Details
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        `;

    document
        .getElementById("ordersList")
        .insertAdjacentHTML("beforeend", orderCard);
});

connection.start()
    .then()
    .catch(function (err) {
        return console.error(err.toString());
    });

function GetStatusBadgeClass(status) {

    switch (status) {
        case "Pending":
            return "bg-warning"
        case "InProgress":
            return "bg-primary";
        case "Completed":
            return "bg-success";
        case "Cancelled":
            return "bg-danger";
        case "OnHold":
            return "bg-secondary";
        case "Failed":
            return "bg-danger";
        case "Delayed":
            return "bg-danger";
        default:
            return "bg-light text-dark";
    }
}