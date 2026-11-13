(function () {
  const cd = document.getElementById('countdown');
  if (!cd) return;
  let deadline = localStorage.getItem('flash_deadline');
  if (!deadline || Number(deadline) < Date.now()) {
    deadline = Date.now() + 3 * 24 * 60 * 60 * 1000;
    localStorage.setItem('flash_deadline', deadline);
  }
  const pad = n => String(n).padStart(2, '0');
  function tick() {
    const r = Math.max(0, Number(deadline) - Date.now());
    cd.querySelector('#cd-days').textContent = pad(Math.floor(r / 86400000));
    cd.querySelector('#cd-hours').textContent = pad(Math.floor((r / 3600000) % 24));
    cd.querySelector('#cd-mins').textContent = pad(Math.floor((r / 60000) % 60));
    cd.querySelector('#cd-secs').textContent = pad(Math.floor((r / 1000) % 60));
  }
  tick(); setInterval(tick, 1000);

  const btn = document.getElementById('copy-coupon-btn');
  btn?.addEventListener('click', () => {
    navigator.clipboard.writeText('SALE30').then(() => {
      if (window.Toast) Toast.success('Đã sao chép!', 'Mã SALE30 đã được copy');
    });
  });
})();