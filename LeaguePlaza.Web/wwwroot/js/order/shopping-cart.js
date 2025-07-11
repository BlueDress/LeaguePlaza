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
                await createOrder(e);
            }
            if (e.target.parentElement.classList.contains('remove-cart-item-js')) {
                await removeCartItem(e);
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
        const orderInformationInputs = document.querySelectorAll('.order-information-input-js');

        const countryInput = document.querySelector('#country');
        const cityInput = document.querySelector('#city');
        const streetInput = document.querySelector('#street');
        const postalCodeInput = document.querySelector('#postal-code');

        orderInformationInputs?.forEach(i => i.addEventListener('blur', e => handleOrderInformationElementInput(e.target)));

        if (inputValuesAreValid([countryInput, cityInput, streetInput, postalCodeInput])) {
            const orderInformation = {
                country: countryInput.value,
                city: cityInput.value,
                street: streetInput.value,
                postalCode: postalCodeInput.value,
                additionalInformation: document.querySelector('#additional-information')?.value
            };

            const queryParams = new URLSearchParams(orderInformation).toString();

            const response = await fetch(baseUrl + 'showsubmitorder' + `?${queryParams}`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json',
                },
            });

            if (response.status == 200) {
                const orderInformationView = await response.text();
                container.innerHTML = orderInformationView;
            }
        }
    }

    async function createOrder(e) {
        const orderInformation = {
            country: document.querySelector('#order-information-country').textContent,
            city: document.querySelector('#order-information-city').textContent,
            street: document.querySelector('#order-information-street').textContent,
            postalCode: document.querySelector('#order-information-postal-code').textContent,
            additionalInformation: document.querySelector('#order-information-additional-information').textContent
        };

        const response = await fetch(baseUrl + 'createorder', {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify(orderInformation)
        });

        if (response.status == 200) {
            const orderInformationView = await response.text();
            container.innerHTML = orderInformationView;
        }
    }

    async function removeCartItem(e) {
        const cartItemId = e.target.parentElement.dataset.cartItemId;

        const response = await fetch(baseUrl + 'removecartitem', {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
            },
            body: cartItemId
        });

        if (response.status == 200) {
            const cartItemsView = await response.text();
            container.innerHTML = cartItemsView;
        }
    }

    function inputValuesAreValid(inputs) {
        let result = true;

        for (const input of inputs) {
            const span = input.nextElementSibling;

            if (elementInputIsValid(input)) {
                hideErrorMessage(span);
                continue;
            }

            displayErrorMessage(span, `${input.name} is required`);
            result = false;
        }

        return result;
    }

    function handleOrderInformationElementInput(element) {
        const span = element.nextElementSibling;

        if (elementInputIsValid(element)) {
            hideErrorMessage(span);
            return;
        }

        displayErrorMessage(span, `${element.name} is required`);
    }

    function elementInputIsValid(element) {
        return !element.hasAttribute('required') || element.value;
    }

    function displayErrorMessage(element, message) {
        element.classList.add('error-message');
        element.classList.remove('display-none');
        element.innerText = message;
    }

    function hideErrorMessage(element) {
        element.classList.remove('error-message');
        element.classList.add('display-none');
        element.innerText = '';
    }
}