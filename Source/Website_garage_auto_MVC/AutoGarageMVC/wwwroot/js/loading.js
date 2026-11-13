document.addEventListener('DOMContentLoaded', () => {
  const loadingScreen = document.getElementById('loading-screen');
  const mainSite = document.getElementById('main-site');

  if (!loadingScreen) return;

  const hasEntered = sessionStorage.getItem('autogarage_entered') === '1';

  if (hasEntered) {
    loadingScreen.style.display = 'none';
    if (mainSite) {
      mainSite.classList.remove('main-site-hidden');
      mainSite.classList.add('show');
    }
    return;
  }

  const enterBtn = document.getElementById('enter-btn');
  const carContainer = document.querySelector('.car-container');

  if (enterBtn && carContainer) {
    enterBtn.addEventListener('click', () => {
      enterBtn.style.opacity = '0';
      enterBtn.style.pointerEvents = 'none';
      carContainer.classList.add('drive');

      setTimeout(() => {
        loadingScreen.classList.add('exit');
        if (mainSite) {
          mainSite.classList.remove('main-site-hidden');
          mainSite.classList.add('show');
        }
        sessionStorage.setItem('autogarage_entered', '1');

        setTimeout(() => {
          loadingScreen.style.display = 'none';
        }, 900);
      }, 2200);
    });
  }
});
