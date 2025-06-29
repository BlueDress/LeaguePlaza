document.addEventListener("DOMContentLoaded", viewProductMain());

function viewProductMain() {
    const baseUrl = '/api/orderapi/';

    const minusBtn = document.querySelector('#minus-btn');
    const plusBtn = document.querySelector('#plus-btn');
    const addToCartBtn = document.querySelector('#add-to-cart-btn');
    const quantityInput = document.querySelector('#product-quantity');

    minusBtn.addEventListener('click', e => decreaseQuantity(e));
    plusBtn.addEventListener('click', e => increaseQuantity(e));
    addToCartBtn.addEventListener('click', e => addProductToCart(e));

    function decreaseQuantity(e) {
        if (quantityInput.value > 1) {
            quantityInput.value--;
        }
    }

    function increaseQuantity(e) {
        if (quantityInput.value < 99) {
            quantityInput.value++;
        }
    }

    async function addProductToCart(e) {
        addToCartBtn.setAttribute('disabled', 'disabled');
        e.preventDefault();

        const productId = e.target.dataset.productId;
        const quantity = quantityInput.value;

        const addToCartData = {
            productId: productId,
            quantity: quantity
        }

        const addToCartResponse = await fetch(baseUrl + 'addtocart', {
            method: 'POST',
            headers: {
                'content-type': 'application/json',
            },
            body: JSON.stringify(addToCartData)
        });

        if (addToCartResponse.status == 200) {
            quantityInput.value = 1;

            const getCartItemsCountResponse = await fetch(baseUrl + 'getcartitemscount');

            if (getCartItemsCountResponse.status == 200) {
                const cartCountBadge = document.querySelector('.cart-nav-icon > span');
                cartCountBadge.textContent = await getCartItemsCountResponse.text();
            }

            const addToCartMessageElement = document.querySelector('.add-to-cart-message-js');
            const addToCartResult = await addToCartResponse.json();

            addToCartMessageElement.innerText = addToCartResult.addToCartMessage;

            if (addToCartResult.isAddToCartSuccessful) {
                addToCartMessageElement.classList.add('success-message');
                addToCartMessageElement.classList.remove('display-none');

                setTimeout(() => {
                    addToCartMessageElement.classList.add('display-none');
                    addToCartMessageElement.classList.remove('success-message');
                    addToCartMessageElement.innerText = '';
                }, 3000);
            } else {
                addToCartMessageElement.classList.add('error-message');
                addToCartMessageElement.classList.remove('display-none');

                setTimeout(() => {
                    addToCartMessageElement.classList.add('display-none');
                    addToCartMessageElement.classList.remove('error-message');
                    addToCartMessageElement.innerText = '';
                }, 3000);
            }
        }

        if (addToCartResponse.status == 400) {
            const addToCartMessageElement = document.querySelector('.add-to-cart-message-js');
            addToCartMessageElement.innerText = 'Something went wrong';
            addToCartMessageElement.classList.add('error-message');
            addToCartMessageElement.classList.remove('display-none');

            setTimeout(() => {
                addToCartMessageElement.classList.add('display-none');
                addToCartMessageElement.classList.remove('error-message');
                addToCartMessageElement.innerText = '';
            }, 3000);
        }

        addToCartBtn.removeAttribute('disabled');
    }
}