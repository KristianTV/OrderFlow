var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/order")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveOrderUpdate", (indexOrder) => {
    var noOrdersMessage = document.getElementById("ordersEmpty");
    if (noOrdersMessage) {
        noOrdersMessage.classList.add("d-none");
    }

    const container = document.getElementById("ordersList");

    const col = createElement('div', { className: 'col' }, container);

    const card = createElement('div', { className: 'card h-100 shadow-sm border-primary' }, col);

    const cardBody = createElement('div', { className: 'card-body d-flex flex-column' }, card);

    createElement('h5', {
        className: 'card-title text-primary mb-2',
        innerHTML: `<i class="fas fa-box-open me-2"></i> Order ID: ${indexOrder.orderID}`
    }, cardBody);

    createElement('h6', {
        className: 'card-subtitle mb-3 text-muted',
        innerHTML: `<i class="far fa-calendar-alt me-1"></i> Date: ${indexOrder.orderDate}`
    }, cardBody);

    createElement('p', {
        className: 'card-text mb-1',
        innerHTML: `<strong><i class="fas fa-truck-loading me-1"></i> Delivery:</strong> ${indexOrder.deliveryAddress}`
    }, cardBody);

    createElement('p', {
        className: 'card-text mb-3',
        innerHTML: `<strong><i class="fas fa-map-marker-alt me-1"></i> Pickup:</strong> ${indexOrder.pickupAddress}`
    }, cardBody);

    const statusPara = createElement('p', { className: 'card-text mt-auto' }, cardBody);
    createElement('strong', { textContent: 'Status: ' }, statusPara);
    createElement('span', {
        className: `badge ${GetStatusBadgeClass(indexOrder.status)} fs-6`,
        textContent: indexOrder.status
    }, statusPara);

    const cardFooter = createElement('div', { className: 'card-footer text-end bg-light border-top-0' }, card);

    createElement('a', {
        className: 'btn btn-primary',
        href: `/Admin/Order/Detail/${indexOrder.orderID}`,
        innerHTML: `<i class="fas fa-info-circle me-2"></i> View Details`
    }, cardFooter);
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