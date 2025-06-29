document.addEventListener("DOMContentLoaded", orderHistoryMain());

function orderHistoryMain() {
    const baseUrl = '/api/orderapi/';

    const orderHistoryWithPagination = document.querySelector('#order-history-and-pagination');

    orderHistoryWithPagination.addEventListener('click', e => getOrderHistoryPageResults(e));

    async function getOrderHistoryPageResults(e) {

        if (e.target && e.target.classList.contains('pagination-button-js')) {
            const pageNumber = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination')?.dataset.value ?? 1;

            const response = await fetch(baseUrl + 'getpageresults' + `?pageNumber=${pageNumber}`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json',
                },
            });

            if (response.status == 200) {
                const orderHistoryWithPaginationView = await response.text();
                orderHistoryWithPagination.innerHTML = orderHistoryWithPaginationView;
            }
        }
    }
}