document.addEventListener("DOMContentLoaded", filterAndSortQuestsMain());

function filterAndSortQuestsMain() {
    const baseUrl = '/api/questapi/';

    const applyFiltersBtn = document.querySelector('#apply-filters');
    const searchQuestForm = document.querySelector('#search-quest');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');

    const statusFilterInputs = document.querySelectorAll('#status-filters input');
    const typeFilterInputs = document.querySelectorAll('#type-filters input');
    const sortBySelect = document.querySelector('#sort-by');
    const searchInput = document.querySelector('#search');

    cardsAndPaginationHolder.addEventListener('click', e => handlePaginationClick(e));
    applyFiltersBtn.addEventListener('click', e => filterAndSortQuests(e));
    searchQuestForm.addEventListener('submit', e => filterAndSortQuests(e));
    sortBySelect.addEventListener('blur', e => filterAndSortQuests(e));

    function handlePaginationClick(e) {
        if (e.target && e.target.classList.contains('pagination-button-js')) {
            filterAndSortQuests(e);
        }
    }

    async function filterAndSortQuests(e) {
        e.preventDefault();

        const searchTerm = searchInput.value;
        const sortBy = getSortByValue();
        const orderIsDescending = getOrderDirection();
        const statusFilters = Array.from(statusFilterInputs).filter(input => input.checked).map(input => input.dataset.value);
        const typeFilters = Array.from(typeFilterInputs).filter(input => input.checked).map(input => input.dataset.value);
        const currentPage = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination').dataset.value;

        const filterAndSortData = {
            searchTerm: searchTerm,
            sortBy: sortBy,
            orderIsDescending: orderIsDescending,
            statusFilters: statusFilters,
            typeFilters: typeFilters,
            currentPage: currentPage
        }

        const queryParams = new URLSearchParams(filterAndSortData).toString();

        const response = await fetch(baseUrl + 'filterandsortquests' + `?${queryParams}`, {
            method: 'GET',
            headers: {
                'content-type': 'application/json',
            },
        });

        if (response.status == 200) {
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
        }

        // TODO: handle server error
    }

    function getSortByValue() {
        if (sortBySelect.value == 3 || sortBySelect.value == 4) {
            return "Reward";
        }

        return "Date";
    }

    function getOrderDirection() {
        return !(sortBySelect.value == 2 || sortBySelect.value == 4);
    }
}