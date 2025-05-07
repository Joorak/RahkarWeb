function scrollContainer(direction) {
    const container = document.querySelector('.horizontal-scroll-container');
    const scrollAmount = 150; 
    container.scrollBy({
        left: direction * scrollAmount,
        behavior: 'smooth'
    });
}