document.addEventListener("DOMContentLoaded", mountRentHistoryMain());

function mountRentHistoryMain() {
    const baseUrl = '/api/mountapi/';

    const mountRentCancelBtns = document.querySelectorAll('.cancel-rent-btn-js');

    mountRentCancelBtns.forEach(btn => btn.addEventListener('click', e => cancelMountRent(e)));

    async function cancelMountRent(e) {
        e.preventDefault();

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