function escapeHtml(str) {
  const d = document.createElement('div');
  d.textContent = str == null ? '' : String(str);
  return d.innerHTML;
}

/* ===== RENDER XE VÀ PHỤ KIỆN (LẤY TỪ LOCALSTORAGE DO ADMIN NHẬP) ===== */
// Hàm hỗ trợ render ảnh (Xử lý cả Emoji và Ảnh Base64)
function renderImage(imgSrc, extraStyle = "") {
  if (imgSrc && imgSrc.startsWith('data:image')) {
    return `<img src="${imgSrc}" style="width:100%; height:100%; object-fit:cover; ${extraStyle}">`;
  }
  return `<span style="${extraStyle}">${imgSrc || '📦'}</span>`;
}

function renderCars(list) {
  const grid = document.getElementById('cars-grid');
  if (!grid) return;
  if (!list || !list.length) {
    grid.innerHTML = '<p style="color:#888;grid-column:1/-1;padding:20px">Không có xe nào.</p>';
    return;
  }
  grid.innerHTML = list.map(car => `
    <a class="card" href="product.html?type=car&id=${car.id}">
      <div class="card-img" style="background:#fff; display:flex; align-items:center; justify-content:center;">
        ${renderImage(car.img, 'font-size:60px')}
      </div>
      <div class="card-body">
        <div class="card-brand">${escapeHtml(car.brand)}</div>
        <div class="card-title">${escapeHtml(car.name)}</div>
        <div class="card-price">${escapeHtml(car.price)}</div>
      </div>
    </a>
  `).join('');
}

function renderAccessories(list) {
  const grid = document.getElementById('acc-grid');
  if (!grid) return;
  if (!list || !list.length) {
    grid.innerHTML = '<p style="color:#888;grid-column:1/-1;padding:20px">Không có phụ kiện nào.</p>';
    return;
  }
  grid.innerHTML = list.map(acc => `
    <a class="card" href="product.html?type=acc&id=${acc.id}">
      <div class="card-img" style="background:#fff; display:flex; align-items:center; justify-content:center;">
        ${renderImage(acc.img, 'font-size:60px')}
      </div>
      <div class="card-body">
        <div class="card-brand">${escapeHtml(acc.category)}</div>
        <div class="card-title">${escapeHtml(acc.name)}</div>
        <div class="card-price">${escapeHtml(acc.price)}</div>
      </div>
    </a>
  `).join('');
}

/* ===== COUNTDOWN FLASH SALE ===== */
function initCountdown() {
  const cd = document.getElementById('countdown');
  if (!cd) return;

  let deadline = localStorage.getItem('flash_deadline');
  if (!deadline || Number(deadline) < Date.now()) {
    deadline = Date.now() + 3 * 24 * 60 * 60 * 1000;
    localStorage.setItem('flash_deadline', deadline);
  }

  const pad = n => String(n).padStart(2, '0');
  let timer;

  function tick() {
    const remain = Math.max(0, Number(deadline) - Date.now());
    const days = Math.floor(remain / (1000 * 60 * 60 * 24));
    const hours = Math.floor((remain / (1000 * 60 * 60)) % 24);
    const mins = Math.floor((remain / (1000 * 60)) % 60);
    const secs = Math.floor((remain / 1000) % 60);

    document.getElementById('cd-days').textContent = pad(days);
    document.getElementById('cd-hours').textContent = pad(hours);
    document.getElementById('cd-mins').textContent = pad(mins);
    document.getElementById('cd-secs').textContent = pad(secs);

    if (remain <= 0) {
      clearInterval(timer);
      cd.innerHTML = '<p style="color:#ffcc00;font-size:18px;font-weight:700">Flash Sale đã kết thúc!</p>';
    }
  }
  tick();
  timer = setInterval(tick, 1000);
}

function initCouponCopy() {
  const btn = document.getElementById('copy-coupon-btn');
  if (!btn) return;
  btn.addEventListener('click', () => {
    navigator.clipboard.writeText('SALE30').then(() => {
      if (typeof Toast !== 'undefined') {
        Toast.success('Đã sao chép!', 'Mã SALE30 đã được copy vào clipboard');
      }
      btn.textContent = 'Đã sao chép!';
      setTimeout(() => btn.textContent = 'Sao chép mã', 1800);
    }).catch(() => {
      if (typeof Toast !== 'undefined') {
        Toast.error('Không thể sao chép', 'Vui lòng copy thủ công: SALE30');
      }
    });
  });
}

/* ===== HEADER: badge giỏ hàng + trạng thái đăng nhập ===== */
function initHeader() {
  const cartCountEl = document.getElementById('cart-count');
  if (cartCountEl) {
    const cart = JSON.parse(localStorage.getItem('autogarage_cart') || '[]');
    cartCountEl.textContent = cart.reduce((s, i) => s + i.qty, 0);
  }

  const authBtn = document.getElementById('header-auth-btn');
  const current = JSON.parse(localStorage.getItem('autogarage_current') || 'null');
  if (current && authBtn) {
    // Nếu là Admin thì trỏ về admin.html, ngược lại về profile.html
    authBtn.href = current.role === 'admin' ? 'admin.html' : 'profile.html';
    authBtn.innerHTML = `
      <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
        <circle cx="12" cy="7" r="4"></circle>
      </svg>
      ${escapeHtml(current.name.split(' ').pop())}
    `;
  }
}

/* ===== INIT ===== */
function initGarage() {
  initCountdown();
  initCouponCopy();
  initHeader();
}

if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initGarage);
} else {
  initGarage();
}

const newsletterForm = document.getElementById('newsletter-form');
if (newsletterForm) {
  newsletterForm.addEventListener('submit', e => {
    e.preventDefault();
    if (typeof Toast !== 'undefined') {
      Toast.success('Đăng ký thành công!', 'Cảm ơn bạn đã đăng ký nhận tin từ AutoGarage');
    }
    newsletterForm.reset();
  });
}