document.addEventListener("DOMContentLoaded", navigationMain());

function navigationMain() {
    const baseOrderUrl = '/api/orderapi/';

    const hamburgerMenu = document.querySelector('#hamburger-menu');
    const cartCountBadge = document.querySelector('.cart-nav-icon > span');

    hamburgerMenu.addEventListener('click', e => handleHamburgerMenuClick(e));

    if (cartCountBadge) {
        updateCartCountBadge();
    }

    function handleHamburgerMenuClick(e) {
        const menuOpen = document.querySelector('#menu-open');
        const menuClose = document.querySelector('#menu-close');
        const navLinks = document.querySelector('#nav-links');

        if (e.target.id === 'menu-open') {
            menuOpen.classList.add('display-none');
            menuClose.classList.remove('display-none');
            navLinks.classList.add('open');
        }

        if (e.target.id === 'menu-close') {
            menuOpen.classList.remove('display-none');
            menuClose.classList.add('display-none');
            navLinks.classList.remove('open');
        }
    }

    async function updateCartCountBadge() {
        const response = await fetch(baseOrderUrl + 'getcartitemscount');

        if (response.status == 200) {
            cartCountBadge.textContent = await response.text();
        }
    }
}