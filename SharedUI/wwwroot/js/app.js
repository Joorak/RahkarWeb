function changeCulture(culture) {
    if (culture === 'fa-IR') culture = 'en-US';
    else culture = 'fa-IR';

    localStorage.setItem('blazorCulture', culture);

    location.reload();
}

function flagClick(culture) {
    let flag = document.querySelector('.flag');
    if (flag.classList.contains('flag-us')) {
        flag.classList.remove('flag-us');
        flag.classList.add('flag-ir');
    } else {
        flag.classList.remove('flag-ir');
        flag.classList.add('flag-us');
    }
    changeCulture(culture);
}
let sliderInterval;
async function initHome(dotNetObjRef, interval) {
    if (document.getElementById("home-video-bg")) {
        let base64String = "data:video/webm;base64,GkXfo5...";
        document.getElementById("home-video-bg").src = base64String;
    }

    startSlider(dotNetObjRef, interval);
};
async function startSlider(dotNetObjRef, interval) {
    window.blazorSlider = dotNetObjRef;
    sliderInterval = setInterval(() => {
        window.blazorSlider.invokeMethodAsync("ShowSlide");
    }, interval);
};
async function stopSlider() {
    clearInterval(sliderInterval);
};
let carouselInterval;
async function startCarousel() {
    const carouselTracker = document.querySelector(".multi-carousel-tracker");
    const CarouselItems = document.querySelectorAll(".multi-carousel-item");
    let index = 0;

    carouselInterval = setInterval(() => {
        index++;
        if (index > (CarouselItems.length - 4)) index = 0; // Reset if end reached
        if (document.querySelector("#page-container").getAttribute('dir') === 'rtl')
            carouselTracker.style.transform = `translateX(${index * 25}%)`;
        else
            carouselTracker.style.transform = `translateX(-${index * 25}%)`;
    }, 3000);
};
async function stopCarousel() {
    clearInterval(carouselInterval);
};


async function initAboutus() {
    //var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    //var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    //    return new bootstrap.Tooltip(tooltipTriggerEl)
    //})


    const video = document.getElementById("video");
    const playButton = document.getElementById("playButton");
    const muteButton = document.getElementById("muteButton");

    if (video) {
        video.muted = true;
        var isPlaying = video.currentTime > 0 && !video.paused && !video.ended
            && video.readyState > video.HAVE_CURRENT_DATA;

        if (!isPlaying) {
            video.play();
        }
        video.addEventListener("click", () => {
            if (video.paused) {
                video.play();
                playButton.style.display = "none";
            } else {
                video.pause();
                playButton.style.display = "block";
            }
        });

        playButton.addEventListener("click", () => {
            video.play();
            playButton.style.display = "none";
        });

        muteButton.addEventListener("click", () => {
            video.muted = !video.muted;
            muteButton.textContent = video.muted ? "🔇" : "🔊";
        });
    }

    document.querySelectorAll('.intro [data-appear-animation]').forEach(function (el) {


        let delay = el.getAttribute('data-appear-animation-delay') || 0;
        let duration = el.getAttribute('data-appear-animation-duration') || '2000';
        const animationType = el.getAttribute('data-appear-animation') || '';

        //if (duration !== '750ms') {
        //    el.style.animationDuration = duration;
        //}
        el.style.animationDuration = `${duration}ms`;
        el.style.animationDelay = `${delay}ms`;
        //el.style.animatuinNmame = animationName;
        if (animationType) 
            el.classList.add(animationType);
        //} else {
        //    el.classList.add('appear-animation-visible');
        //}
    });

    const timelineItems = document.querySelectorAll('.timeline [data-appear-animation]');
    
    window.addEventListener('scroll', function (event) {
        timelineItems.forEach(function (el) {
            if (isInViewport(el)) {
                //let opts = el.dataset;
                //el.classList.add('appear-animation', 'animated');

                let delay = el.getAttribute('data-appear-animation-delay') || 0;
                let duration = el.getAttribute('data-appear-animation-duration') || '2000';
                const animationType = el.getAttribute('data-appear-animation') || '';

                el.style.animationDuration = `${duration}ms`;
                el.style.animationDelay = `${delay}ms`;

                if (animationType)
                    el.classList.add(animationType);
            }
        });
    });
};

function isInViewport(element) {
    const { top, bottom } = element.getBoundingClientRect();
    const vHeight = (window.innerHeight || document.documentElement.clientHeight);

    return (
        (top > 0 || bottom > 0) &&
        top + ((bottom - top) / 2) < vHeight
    );
};
function forceRedraw(element) {

    if (!element) { return; }

    var n = document.createTextNode(' ');
    n.clientWidth = element.clientWidth;
    n.clientHeight = element.clientHeight;
    var disp = element.style.display;  // don't worry about previous display style

    element.appendChild(n);
    element.style.display = 'none';

    setTimeout(function () {
        element.style.display = disp;
        n.parentNode.removeChild(n);
    }, 20); // you can play with this timeout to make it as short as possible
};

function updateContent(index, eventh, eventp) {
    var dir = document.getElementById('page-container').getAttribute('dir');
    var container = document.getElementById('timeline-h-content-container');

    if (!container) return;

    container.classList.remove('slide-in-right', 'slide-in-left', 'slide-out-left', 'slide-out-right');
    container.classList.add(dir == 'rtl' ? 'slide-out-left' : 'slide-out-right');



    setTimeout(() => {
        document.getElementById('content-title').textContent = eventh;
        document.getElementById('content-text').textContent = eventp;

        container.classList.remove('slide-in-right', 'slide-in-left', 'slide-out-left', 'slide-out-right');
        container.classList.add(dir == 'rtl' ? 'slide-in-right' : 'slide-in-left');
    }, 50);

    document.querySelectorAll('.event').forEach(el => el.classList.remove('active'));
    const activeEvent = document.querySelectorAll('.event')[index];
    activeEvent.classList.add('active');
    const timelineLine = document.getElementsByClassName('timeline-h-line')[0];
    timelineLine.style.background = `linear-gradient(to right,rgba(223, 223, 223, 0) 1%, rgba(223, 223, 223, 1) 14.99%, rgba(122, 157, 6, 0) 15%, rgba(122, 157, 6, 1) ${activeEvent.style.left}, rgba(223, 223, 223, 1) ${activeEvent.style.left}, rgba(223, 223, 223, 0))`;
};



function resizeIframe(height,url) {
    try {
        var iframe = document.getElementById('about-team');
        if (!iframe) return;
        if (height == "none") {
            window.location = url;
            return;
        }
        iframe.style.height = height + "px";
        //iframedoc = iframe.contentDocument || iframe.contentWindow.document;
        //iframe.style.height = iframedoc.body.scrollHeight
    } catch (e) {
        System.Console.log(e);
        window.history.go(-1);
        OpenInNewWindow(url, 'please wait...');
        
    }
    
};
function setIframeHeight (desktopHeight, mobileHeight) {
    if (document.body.style.width < 768){
        document.getElementById('about-team').style.height = mobileHeight + 'px';
    }
    else {
        document.getElementById('about-team').style.height = desktopHeight + 'px';
    }
}
function OpenInNewWindow(url, message) {
    var newwindow = window.open('', '_blank');
    newwindow.document.write(message);
    newwindow.location.href = url;
}