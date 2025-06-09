document.addEventListener("DOMContentLoaded", viewMountMain());

function viewMountMain() {
    const baseUrl = '/api/mountapi/';

    const rentMountBtn = document.querySelector('#rent-mount');
    const rateMountSelect = document.querySelector('#rating-select');

    const startDateInput = document.querySelector('#start-date');
    const endDateInput = document.querySelector('#end-date');

    rentMountBtn.addEventListener('click', e => rentMount(e));
    rateMountSelect.addEventListener('change', e => rateMount(e));

    async function rentMount(e) {
        const startDate = startDateInput.value;
        const endDate = endDateInput.value;

        if (inputDateIntervalIsNotValid(startDate, endDate)) {
            const dateInputsMessageElement = document.querySelector('.date-inputs-message-js');
            dateInputsMessageElement.innerText = 'Date interval is not valid';
            dateInputsMessageElement.classList.add('error-message');
            dateInputsMessageElement.classList.remove('display-none');

            setTimeout(() => {
                dateInputsMessageElement.classList.add('display-none');
                dateInputsMessageElement.classList.remove('error-message');
                dateInputsMessageElement.innerText = '';
            }, 3000);

            return;
        }

        const rentMountDto = {
            mountId: e.target.dataset.mountId,
            startDate: startDate,
            endDate: endDate
        }

        const response = await fetch(baseUrl + 'rentmount', {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify(rentMountDto)
        });

        if (response.status == 200) {
            const dateInputsMessageElement = document.querySelector('.date-inputs-message-js');
            const mountRentResult = await response.json();

            dateInputsMessageElement.innerText = mountRentResult.mountRentMessage;

            if (mountRentResult.isMountRentSuccessful) {
                dateInputsMessageElement.classList.add('success-message');
                dateInputsMessageElement.classList.remove('display-none');

                setTimeout(() => {
                    dateInputsMessageElement.classList.add('display-none');
                    dateInputsMessageElement.classList.remove('success-message');
                    dateInputsMessageElement.innerText = '';
                }, 3000);
            } else {
                dateInputsMessageElement.classList.add('error-message');
                dateInputsMessageElement.classList.remove('display-none');

                setTimeout(() => {
                    dateInputsMessageElement.classList.add('display-none');
                    dateInputsMessageElement.classList.remove('error-message');
                    dateInputsMessageElement.innerText = '';
                }, 3000);
            }
        }

        if (response.status == 400) {
            const dateInputsMessageElement = document.querySelector('.date-inputs-message-js');
            dateInputsMessageElement.innerText = 'Something went wrong';
            dateInputsMessageElement.classList.add('error-message');
            dateInputsMessageElement.classList.remove('display-none');

            setTimeout(() => {
                dateInputsMessageElement.classList.add('display-none');
                dateInputsMessageElement.classList.remove('error-message');
                dateInputsMessageElement.innerText = '';
            }, 3000);
        }
    }

    async function rateMount(e) {
        const rateMountDto = {
            mountId: e.target.dataset.mountId,
            rating: e.target.value
        }

        const response = await fetch(baseUrl + 'ratemount', {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify(rateMountDto)
        });

        if (response.status == 200) {
            const dateInputsMessageElement = document.querySelector('.rating-select-message-js');
            dateInputsMessageElement.innerText = await response.text();
            dateInputsMessageElement.classList.remove('display-none');

            setTimeout(() => {
                dateInputsMessageElement.classList.add('display-none');
                dateInputsMessageElement.innerText = '';
            }, 3000);
        }

        if (response.status == 400) {
            const dateInputsMessageElement = document.querySelector('.rating-select-message-js');
            dateInputsMessageElement.innerText = 'Something went wrong';
            dateInputsMessageElement.classList.add('error-message');
            dateInputsMessageElement.classList.remove('display-none');

            setTimeout(() => {
                dateInputsMessageElement.classList.add('display-none');
                dateInputsMessageElement.classList.remove('error-message');
                dateInputsMessageElement.innerText = '';
            }, 3000);
        }
    }

    function inputDateIntervalIsNotValid(startDate, endDate) {
        return !((startDate && endDate && startDate <= endDate));
    }
}