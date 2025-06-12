document.addEventListener("DOMContentLoaded", filterAndSortQuestsMain());

function filterAndSortQuestsMain() {
    const baseUrl = '/api/questapi/';

    const applyFiltersBtn = document.querySelector('#apply-filters');
    const resetFiltersBtn = document.querySelector('#reset-filters');
    const searchQuestForm = document.querySelector('#search-quest');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');

    const statusFilterInputs = document.querySelectorAll('#status-filters input');
    const typeFilterInputs = document.querySelectorAll('#type-filters input');
    const sortBySelect = document.querySelector('#sort-by');
    const searchInput = document.querySelector('#search');

    const pageName = document.querySelector('.container').dataset.pageName;

    cardsAndPaginationHolder.addEventListener('click', e => handlePaginationClick(e));
    resetFiltersBtn.addEventListener('click', e => resetFiltersAndSearch(e));
    applyFiltersBtn.addEventListener('click', e => filterAndSortQuests(e));
    searchQuestForm.addEventListener('submit', e => filterAndSortQuests(e));
    sortBySelect.addEventListener('change', e => filterAndSortQuests(e));

    hideStatusFiltersForAllQuestsPage();

    function handlePaginationClick(e) {
        if (e.target && e.target.classList.contains('pagination-button-js')) {
            filterAndSortQuests(e);
        }
    }

    function resetFiltersAndSearch(e) {
        statusFilterInputs.forEach(f => f.checked = false);
        typeFilterInputs.forEach(f => f.checked = false);
        sortBySelect.value = 1;
        searchInput.value = '';
        filterAndSortQuests(e);
    }

    async function filterAndSortQuests(e) {
        e.preventDefault();

        const searchTerm = searchInput.value;
        const sortBy = getSortByValue();
        const orderIsDescending = getOrderDirection();
        const statusFilters = Array.from(statusFilterInputs).filter(input => input.checked).map(input => input.dataset.value).join(',');
        const typeFilters = Array.from(typeFilterInputs).filter(input => input.checked).map(input => input.dataset.value).join(',');
        const currentPage = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination')?.dataset.value ?? 1;
        const pageIsMyQuests = document.querySelector('.container').dataset.pageName === 'my-quests';

        const filterAndSortData = {
            searchTerm: searchTerm,
            sortBy: sortBy,
            orderIsDescending: orderIsDescending,
            statusFilters: statusFilters,
            typeFilters: typeFilters,
            currentPage: currentPage,
            pageIsMyQuests: pageIsMyQuests
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
            return "Reward";
        }

        return "Date";
    }

    function getOrderDirection() {
        return !(sortBySelect.value == 2 || sortBySelect.value == 4);
    }

    function hideStatusFiltersForAllQuestsPage() {
        if (pageName === 'all-quests') {
            document.querySelector('#status-filters').parentElement.classList.add('display-none');
        }
    }
}