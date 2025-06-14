document.addEventListener("DOMContentLoaded", filterAndSortProductsMain());

function filterAndSortProductsMain() {
    const baseUrl = '/api/productapi/';

    const applyFiltersBtn = document.querySelector('#apply-filters');
    const resetFiltersBtn = document.querySelector('#reset-filters');
    const searchProductForm = document.querySelector('#search-product');
    const cardsAndPaginationHolder = document.querySelector('#cards-and-pagination');

    const typeFilterInputs = document.querySelectorAll('#type-filters input');
    const sortBySelect = document.querySelector('#sort-by');
    const searchInput = document.querySelector('#search');

    cardsAndPaginationHolder.addEventListener('click', e => handlePaginationClick(e));
    resetFiltersBtn.addEventListener('click', e => resetFiltersAndSearch(e));
    applyFiltersBtn.addEventListener('click', e => filterAndSortProducts(e));
    searchProductForm.addEventListener('submit', e => filterAndSortProducts(e));
    sortBySelect.addEventListener('change', e => filterAndSortProducts(e));

    function handlePaginationClick(e) {
        if (e.target && e.target.classList.contains('pagination-button-js')) {
            filterAndSortProducts(e);
        }
    }

    function resetFiltersAndSearch(e) {
        typeFilterInputs.forEach(f => f.checked = false);
        sortBySelect.value = 1;
        searchInput.value = '';
        filterAndSortProducts(e);
    }

    async function filterAndSortProducts(e) {
        e.preventDefault();

        const searchTerm = searchInput.value;
        const sortBy = getSortByValue();
        const orderIsDescending = getOrderDirection();
        const typeFilters = Array.from(typeFilterInputs).filter(input => input.checked).map(input => input.dataset.value).join(',');
        const currentPage = e.target.classList.contains('pagination-button-js') ? e.target.dataset.value : document.querySelector('.active-pagination')?.dataset.value ?? 1;

        const filterAndSortData = {
            searchTerm: searchTerm,
            sortBy: sortBy,
            orderIsDescending: orderIsDescending,
            typeFilters: typeFilters,
            currentPage: currentPage
        }

        const queryParams = new URLSearchParams(filterAndSortData).toString();

        const response = await fetch(baseUrl + 'filterandsortproducts' + `?${queryParams}`, {
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

        return "Name";
    }

    function getOrderDirection() {
        return !(sortBySelect.value == 1 || sortBySelect.value == 3);
    }
}