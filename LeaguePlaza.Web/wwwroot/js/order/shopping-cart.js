document.addEventListener("DOMContentLoaded", shoppingCartMain());

function shoppingCartMain() {
    const baseUrl = '/api/orderapi/';

    const container = document.querySelector('.container-js');

    container.addEventListener('click', e => handleShoppingCartButtonClick(e));

    async function handleShoppingCartButtonClick(e) {
        if (e.target) {
            if (e.target.id == 'cart-items-continue') {
                await showOrderInformation(e);
            }
            if (e.target.id == 'order-information-back') {
                await showCartItems(e);
            }
            if (e.target.id == 'order-information-continue') {
                await showSubmitOrder(e);
            }
            if (e.target.id == 'submit-order-back') {
                await showOrderInformation(e);
            }
            if (e.target.id == 'submit-order-confirm') {
                await showOrderSuccessful(e);
            }
        }
    }

    async function showOrderInformation(e) {
        const response = await fetch(baseUrl + 'showorderinformation');

        if (response.status == 200) {
            const orderInformationView = await response.text();
            container.innerHTML = orderInformationView;
        }
    }

    async function showCartItems(e) {
        const response = await fetch(baseUrl + 'showcartitems');

        if (response.status == 200) {
            const orderInformationView = await response.text();
            container.innerHTML = orderInformationView;
        }
    }

    async function showSubmitOrder(e) {
        const response = await fetch(baseUrl + 'showsubmitorder');

        if (response.status == 200) {
            const orderInformationView = await response.text();
            container.innerHTML = orderInformationView;
        }
    }

    async function showOrderSuccessful(e) {
        const response = await fetch(baseUrl + 'showordersuccessful');

        if (response.status == 200) {
            const orderInformationView = await response.text();
            container.innerHTML = orderInformationView;
        }
    }
}