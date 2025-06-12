document.addEventListener("DOMContentLoaded", filterAndSortMountsMain());

function filterAndSortMountsMain() {
    const baseUrl = '/api/mountapi/';

    const applyFiltersBtn = document.querySelector('#apply-filters');
    const resetFiltersBtn = document.querySelector('#reset-filters');
    const checkAvailableBtn = document.querySelector('#check-available');
    const searchMountForm = document.querySelector('#search-mount');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');

    const typeFilterInputs = document.querySelectorAll('#type-filters input');
    const sortBySelect = document.querySelector('#sort-by');
    const searchInput = document.querySelector('#search');
    const startDateInput = document.querySelector('#start-date');
    const endDateInput = document.querySelector('#end-date');

    cardsAndPaginationHolder.addEventListener('click', e => handlePaginationClick(e));
    resetFiltersBtn.addEventListener('click', e => resetFiltersAndSearch(e));
    applyFiltersBtn.addEventListener('click', e => filterAndSortMounts(e));
    checkAvailableBtn.addEventListener('click', e => filterAndSortMounts(e));
    searchMountForm.addEventListener('submit', e => filterAndSortMounts(e));
    sortBySelect.addEventListener('change', e => filterAndSortMounts(e));

    function handlePaginationClick(e) {
        if (e.target && e.target.classList.contains('pagination-button-js')) {
            filterAndSortMounts(e);
        }
    }

    function resetFiltersAndSearch(e) {
        typeFilterInputs.forEach(f => f.checked = false);
        sortBySelect.value = 1;
        searchInput.value = '';
        startDateInput.value = '';
        endDateInput.value = '';
        filterAndSortMounts(e);
    }

    async function filterAndSortMounts(e) {
        e.preventDefault();

        const searchTerm = searchInput.value;
        const sortBy = getSortByValue();
        const orderIsDescending = getOrderDirection();
        const typeFilters = Array.from(typeFilterInputs).filter(input => input.checked).map(input => input.dataset.value).join(',');
        const startDate = startDateInput.value;
        const endDate = endDateInput.value;
        const currentPage = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination')?.dataset.value ?? 1;

        if (inputDateIntervalIsNotValid(startDate, endDate)) {
            const filtersMessageElement = document.querySelector('.filters-message-js');
            filtersMessageElement.innerText = 'Date interval is not valid';
            filtersMessageElement.classList.add('error-message');
            filtersMessageElement.classList.remove('display-none');

            setTimeout(() => {
                filtersMessageElement.classList.add('display-none');
                filtersMessageElement.classList.remove('error-message');
                filtersMessageElement.innerText = '';
            }, 3000);

            return;
        }

        const filterAndSortData = {
            searchTerm: searchTerm,
            sortBy: sortBy,
            orderIsDescending: orderIsDescending,
            typeFilters: typeFilters,
            startDate: startDate,
            endDate: endDate,
            currentPage: currentPage
        }

        const queryParams = new URLSearchParams(filterAndSortData).toString();

        const response = await fetch(baseUrl + 'filterandsortmounts' + `?${queryParams}`, {
            method: 'GET',
            headers: {
                'content-type': 'application/json',
            },
        });

        if (response.status == 200) {
            const cardsAndPaginationHolderView = await response.text();
            cardsAndPaginationHolder.innerHTML = cardsAndPaginationHolderView;
        }

        if (response.status == 400) {
            const filtersMessageElement = document.querySelector('.filters-message-js');
            filtersMessageElement.innerText = 'Something went wrong';
            filtersMessageElement.classList.add('error-message');
            filtersMessageElement.classList.remove('display-none');

            setTimeout(() => {
                filtersMessageElement.classList.add('display-none');
                filtersMessageElement.classList.remove('error-message');
                filtersMessageElement.innerText = '';
            }, 3000);
        }
    }

    function getSortByValue() {
        if (sortBySelect.value == 3 || sortBySelect.value == 4) {
            return "Price";
        }

        return "Rating";
    }

    function getOrderDirection() {
        return !(sortBySelect.value == 2 || sortBySelect.value == 4);
    }

    function inputDateIntervalIsNotValid(startDate, endDate) {
        return !((!startDate && !endDate) || (startDate && endDate && startDate >= new Date().toJSON().slice(0, 10) && startDate <= endDate));
    }
}