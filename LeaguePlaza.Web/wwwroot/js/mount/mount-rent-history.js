document.addEventListener("DOMContentLoaded", mountRentHistoryMain());

function mountRentHistoryMain() {
    const baseUrl = '/api/mountapi/';

    const mountRentHistoryWithPagination = document.querySelector('#mount-rent-history-and-pagination');

    mountRentHistoryWithPagination.addEventListener('click', e => cancelMountRent(e));
    mountRentHistoryWithPagination.addEventListener('click', e => getMountRentHistoryPageResults(e));

    async function cancelMountRent(e) {
        e.preventDefault();

        if (e.target && e.target.classList.contains('cancel-rent-btn-js')) {
            const id = e.target.dataset.mountRentalId;

            const response = await fetch(baseUrl + 'cancelmountrent' + `/${id}`, {
                method: 'DELETE',
            });

            if (response.status == 200) {
                const divToRemove = e.target.closest('.mount-rent-info');
                const mountRentHistory = document.querySelector('.mount-rent-history');
                mountRentHistory.removeChild(divToRemove);
            }
        }
    }

    async function getMountRentHistoryPageResults(e) {
        e.preventDefault();

        if (e.target && e.target.classList.contains('pagination-button-js')) {
            const pageNumber = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination')?.dataset.value ?? 1;

            const response = await fetch(baseUrl + 'getpageresults' + `?pageNumber=${pageNumber}`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json',
                },
            });

            if (response.status == 200) {
                const mountRentHistoryWithPaginationView = await response.text();
                mountRentHistoryWithPagination.innerHTML = mountRentHistoryWithPaginationView;
            }
        }
    }
}