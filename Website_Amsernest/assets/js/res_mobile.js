const menuIcon = document.querySelector('.menu-icon');
if (window.innerWidth <= 700) {
    console.log(menuIcon);
    menuIcon.innerHTML = `<i class="fas fa-bars"></i>`;
}