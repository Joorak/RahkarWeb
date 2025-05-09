function scrollContainer(direction) {
    const container = document.querySelector('.horizontal-scroll-container');
    const scrollAmount = 150; 
    container.scrollBy({
        left: direction * scrollAmount,
        behavior: 'smooth'
    });
}

window.onscroll = function () {
    var showAfter = 150;
    var element = document.getElementById('btn-back-to-top');
    if (window.scrollY > showAfter)
        element.style.display = 'block';
    else
        element.style.display = 'none';
};