document.addEventListener('DOMContentLoaded', function () {
    const scrollY = localStorage.getItem('menuScrollY');
    if (scrollY !== null) {
        window.scrollTo(0, parseInt(scrollY));
        localStorage.removeItem('menuScrollY');
    }
});
window.addEventListener('beforeunload', function () {
    localStorage.setItem('menuScrollY', window.scrollY);
});

function scrollToCategory(categoryId) {
    const targetElement = document.getElementById(categoryId);

    const navHeight = document.querySelector('.menu__category-nav').offsetHeight + document.querySelector('.header__menu-inner').offsetHeight;
    const scrollPosition = targetElement.offsetTop - navHeight;

    window.scrollTo({
        top: scrollPosition,
        behavior: 'smooth'
    });
}