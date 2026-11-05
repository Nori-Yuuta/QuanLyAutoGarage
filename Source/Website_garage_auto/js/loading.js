document.addEventListener('DOMContentLoaded', () => {
  const loadingScreen = document.getElementById('loading-screen');
  const mainSite = document.getElementById('main-site');

  const hasEntered = sessionStorage.getItem('autogarage_entered') === '1';

  if (hasEntered) {
    loadingScreen.remove();
    mainSite.classList.remove('hidden');
    mainSite.classList.add('show');
    return;
  }

  const enterBtn = document.getElementById('enter-btn');
  const carContainer = document.querySelector('.car-container');

  enterBtn.addEventListener('click', () => {
    enterBtn.style.opacity = '0';
    enterBtn.style.pointerEvents = 'none';
    carContainer.classList.add('drive');

    setTimeout(() => {
      loadingScreen.classList.add('exit');
      mainSite.classList.remove('hidden');
      sessionStorage.setItem('autogarage_entered', '1');

      requestAnimationFrame(() => mainSite.classList.add('show'));
      setTimeout(() => loadingScreen.remove(), 900);
    }, 2200);
  });
});