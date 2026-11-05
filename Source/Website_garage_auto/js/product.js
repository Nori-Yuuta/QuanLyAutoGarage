(function () {
  const CART_KEY = 'autogarage_cart';

  // Fallback nếu Toast chưa load
  const notify = (type, title, msg) => {
    if (typeof Toast !== 'undefined') {
      Toast[type] ? Toast[type](title, msg) : Toast.show(type, title, msg);
    } else {
      console.log(`[${type}] ${title}: ${msg}`);
    }
  };

  function escapeHtml(str) {
    const d = document.createElement('div');
    d.textContent = str == null ? '' : String(str);
    return d.innerHTML;
  }

  function loadCart() {
    try { return JSON.parse(localStorage.getItem(CART_KEY)) || []; }
    catch { return []; }
  }
  function saveCart(cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
    updateCartCount();
  }
  function updateCartCount() {
    const el = document.getElementById('cart-count');
    if (el) el.textContent = loadCart().reduce((s, i) => s + i.qty, 0);
  }
  function parsePrice(str) { return Number(String(str).replace(/[^\d]/g, '')) || 0; }

  // Kiểm tra data đã load chưa
  if (typeof carsData === 'undefined' || typeof accessoriesData === 'undefined') {
    document.getElementById('product-detail').innerHTML =
      '<p style="padding:40px;text-align:center;color:#e63946">Lỗi: Không tải được dữ liệu sản phẩm. Vui lòng kiểm tra file js/data.js.</p>';
    return;
  }

  const params = new URLSearchParams(window.location.search);
  const type = params.get('type');
  const id = Number(params.get('id'));

  updateCartCount();

  if (!type || !id) {
    document.getElementById('product-detail').innerHTML =
      '<p style="padding:40px;text-align:center;color:#888">Không tìm thấy sản phẩm. Vui lòng chọn từ trang chủ.</p>';
    return;
  }

  if (type === 'car') renderCarDetail(id);
  else if (type === 'acc') renderAccDetail(id);
  else {
    document.getElementById('product-detail').innerHTML =
      '<p style="padding:40px;text-align:center;color:#888">Loại sản phẩm không hợp lệ.</p>';
  }

  /* ===== XE ===== */
  function renderCarDetail(carId) {
    const car = carsData.find(c => c.id === carId);
    if (!car) {
      document.getElementById('product-detail').innerHTML =
        '<p style="padding:40px;text-align:center;color:#888">Không tìm thấy xe này.</p>';
      return;
    }
    document.title = car.name + ' — AutoGarage';

    document.getElementById('product-detail').innerHTML = `
      <div class="product-detail">
        <div class="product-img">${car.img}</div>
        <div class="product-info">
          <h1>${escapeHtml(car.name)}</h1>
          <p class="product-brand">${escapeHtml(car.brand)}</p>
          <div class="product-price">${escapeHtml(car.price)}</div>
          <p class="product-desc">${escapeHtml(car.desc)}</p>
          <table class="specs-table">
            <tr><td>Năm sản xuất</td><td>${car.year}</td></tr>
            <tr><td>Động cơ</td><td>${escapeHtml(car.engine)}</td></tr>
            <tr><td>Nhiên liệu</td><td>${escapeHtml(car.fuel)}</td></tr>
            <tr><td>Số chỗ</td><td>${car.seats}</td></tr>
            <tr><td>Hộp số</td><td>${escapeHtml(car.transmission)}</td></tr>
            <tr><td>Xuất xứ</td><td>${escapeHtml(car.origin)}</td></tr>
          </table>
          <button class="btn-buy-now" id="contact-car-btn">
            Liên hệ đặt mua xe
          </button>
        </div>
      </div>
    `;

    document.getElementById('contact-car-btn').addEventListener('click', () => {
      notify('info', 'Liên hệ đặt mua', 'Vui lòng gọi hotline 0901 234 567 để được tư vấn');
    });

    renderCompatibleAccessories(car.name);
  }

  function renderCompatibleAccessories(carName) {
    const brandPrefix = carName.split(' ')[0].toLowerCase();
    const compat = accessoriesData.filter(a =>
      a.compat.some(c => c.toLowerCase().includes(brandPrefix))
    );

    const section = document.getElementById('compat-section');
    const grid = document.getElementById('compat-grid');

    if (!compat.length) { section.classList.add('hidden'); return; }
    section.classList.remove('hidden');

    grid.innerHTML = compat.map(acc => `
      <a class="card" href="product.html?type=acc&id=${acc.id}">
        <div class="card-img">${acc.img}</div>
        <div class="card-body">
          <div class="card-brand">${escapeHtml(acc.category)}</div>
          <div class="card-title">${escapeHtml(acc.name)}</div>
          <div class="card-price">${escapeHtml(acc.price)}</div>
        </div>
      </a>
    `).join('');
  }

  /* ===== PHỤ KIỆN ===== */
  function renderAccDetail(accId) {
    const acc = accessoriesData.find(a => a.id === accId);
    if (!acc) {
      document.getElementById('product-detail').innerHTML =
        '<p style="padding:40px;text-align:center;color:#888">Không tìm thấy phụ kiện này.</p>';
      return;
    }
    document.title = acc.name + ' — AutoGarage';

    document.getElementById('product-detail').innerHTML = `
      <div class="product-detail">
        <div class="product-img">${acc.img}</div>
        <div class="product-info">
          <h1>${escapeHtml(acc.name)}</h1>
          <p class="product-brand">${escapeHtml(acc.category)}</p>
          <div class="product-price">${escapeHtml(acc.price)}</div>
          <p class="product-desc">${escapeHtml(acc.desc)}</p>
          <table class="specs-table">
            <tr><td>Chất liệu</td><td>${escapeHtml(acc.material)}</td></tr>
            <tr><td>Bảo hành</td><td>${escapeHtml(acc.warranty)}</td></tr>
            <tr><td>Tương thích</td><td>${acc.compat.map(escapeHtml).join(', ')}</td></tr>
          </table>
          <div class="purchase-row">
            <div class="qty-control">
              <button class="qty-btn" data-action="dec">−</button>
              <input type="number" class="qty-input" id="qty" value="1" min="1" max="99">
              <button class="qty-btn" data-action="inc">+</button>
            </div>
            <button class="btn-add-cart" id="add-cart-btn">
              <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <circle cx="9" cy="21" r="1"></circle>
                <circle cx="20" cy="21" r="1"></circle>
                <path d="M1 1h4l2.6 13.4a2 2 0 0 0 2 1.6h9.7a2 2 0 0 0 2-1.6L23 6H6"></path>
              </svg>
              Thêm vào giỏ
            </button>
          </div>
          <button class="btn-buy-now" id="buy-now-btn">Mua ngay</button>
        </div>
      </div>
    `;

    const qtyInput = document.getElementById('qty');
    document.querySelectorAll('.qty-btn').forEach(btn => {
      btn.addEventListener('click', () => {
        let v = parseInt(qtyInput.value) || 1;
        v = btn.dataset.action === 'inc' ? v + 1 : Math.max(1, v - 1);
        qtyInput.value = v;
      });
    });

    document.getElementById('add-cart-btn').addEventListener('click', () => {
      const qty = parseInt(qtyInput.value) || 1;
      addToCart(acc, qty);
      notify('cart', 'Đã thêm vào giỏ!', `${qty} × ${acc.name}`);
    });

    document.getElementById('buy-now-btn').addEventListener('click', () => {
      const qty = parseInt(qtyInput.value) || 1;
      addToCart(acc, qty);
      notify('cart', 'Đã thêm vào giỏ!', `Chuyển đến giỏ hàng...`);
      setTimeout(() => { window.location.href = 'cart.html'; }, 800);
    });

    document.getElementById('compat-section').classList.add('hidden');
  }

function addToCart(acc, qty) {
  const cart = loadCart();
  const existing = cart.find(i => i.id === acc.id);

  if (existing) {
    existing.qty += qty;
  } else {
    cart.push({
      id: acc.id,
      name: acc.name,
      price: parsePrice(acc.price),
      img: acc.img,
      category: acc.category,
      qty: qty
    });
  }

  // Lưu — dùng đúng key
  localStorage.setItem('autogarage_cart', JSON.stringify(cart));

  // ⚠️ QUAN TRỌNG: log để debug
  console.log('✅ Đã thêm vào giỏ:', cart);
  console.log('📦 localStorage.autogarage_cart =',
    localStorage.getItem('autogarage_cart'));

  updateCartCount();
}
})();