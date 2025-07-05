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

    async function showOrderSuccessful(e) {
        const response = await fetch(baseUrl + 'showordersuccessful');

        if (response.status == 200) {
            const orderInformationView = await response.text();
            container.innerHTML = orderInformationView;
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