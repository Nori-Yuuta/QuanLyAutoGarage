(function () {
  const CART_KEY = 'autogarage_cart';
  const CURRENT_KEY = 'autogarage_current';

  const notify = (type, title, msg) => {
    if (typeof Toast !== 'undefined' && Toast[type]) {
      Toast[type](title, msg);
    } else {
      console.log(`[${type}] ${title}: ${msg}`);
    }
  };

  function loadCart() {
    try {
      const raw = localStorage.getItem(CART_KEY);
      return JSON.parse(raw) || [];
    } catch (e) {
      return [];
    }
  }

  function saveCart(cart) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart));
  }

  function formatPrice(n) {
    return Number(n).toLocaleString('vi-VN') + ' ₫';
  }

  const emptyEl = document.getElementById('cart-empty');
  const contentEl = document.getElementById('cart-content');
  const itemsEl = document.getElementById('cart-items');

  if (!emptyEl || !contentEl || !itemsEl) {
    return;
  }

  // ===== SỬA LỖI: Hàm render ảnh an toàn =====
  function renderCartImage(imgSrc) {
    if (imgSrc && imgSrc.startsWith('data:image')) {
      return `<img src="${imgSrc}" style="width:100%; height:100%; object-fit:cover; border-radius:8px;">`;
    }
    return `<span style="font-size:36px;">${imgSrc || '📦'}</span>`;
  }

  function render() {
    const cart = loadCart();

    if (!cart.length) {
      emptyEl.style.display = 'block';
      contentEl.style.display = 'none';
      return;
    }

    emptyEl.style.display = 'none';
    contentEl.style.display = 'grid';

    itemsEl.innerHTML = cart.map(item => `
      <div class="cart-item" data-id="${item.id}">
        <!-- SỬA LỖI: Render ảnh an toàn -->
        <div class="cart-item-img" style="background:#fff;">${renderCartImage(item.img)}</div>
        <div class="cart-item-info">
          <h4>${item.name}</h4>
          <small>${item.category || ''}</small>
        </div>
        <div class="qty-control">
          <button class="qty-btn" data-action="dec" type="button">−</button>
          <input type="number" class="qty-input" value="${item.qty}" min="1" max="99" readonly>
          <button class="qty-btn" data-action="inc" type="button">+</button>
        </div>
        <div style="text-align:right">
          <div class="cart-item-price">${formatPrice(item.price * item.qty)}</div>
          <button class="cart-item-remove" title="Xóa" type="button">×</button>
        </div>
      </div>
    `).join('');

    updateSummary(cart);

    itemsEl.querySelectorAll('.cart-item').forEach(el => {
      const id = Number(el.dataset.id);
      el.querySelector('[data-action="inc"]').addEventListener('click', () => changeQty(id, 1));
      el.querySelector('[data-action="dec"]').addEventListener('click', () => changeQty(id, -1));
      el.querySelector('.cart-item-remove').addEventListener('click', () => removeItem(id));
    });
  }

  function changeQty(id, delta) {
    const cart = loadCart();
    const item = cart.find(i => i.id === id);
    if (!item) return;
    item.qty = Math.max(1, item.qty + delta);
    saveCart(cart);
    render();
  }

  function removeItem(id) {
    const cart = loadCart().filter(i => i.id !== id);
    saveCart(cart);
    notify('info', 'Đã xóa sản phẩm', 'Sản phẩm đã được xóa khỏi giỏ hàng');
    render();
  }

  function updateSummary(cart) {
    const subtotal = cart.reduce((s, i) => s + i.price * i.qty, 0);
    const shipping = subtotal >= 500000 ? 0 : 30000;
    const total = subtotal + shipping;

    document.getElementById('subtotal').textContent = formatPrice(subtotal);
    document.getElementById('shipping').textContent = shipping === 0 ? 'Miễn phí' : formatPrice(shipping);
    document.getElementById('total').textContent = formatPrice(total);
  }

  const checkoutBtn = document.getElementById('checkout-btn');
  if (checkoutBtn) {
    checkoutBtn.addEventListener('click', () => {
      const current = JSON.parse(localStorage.getItem(CURRENT_KEY) || 'null');
      if (!current) {
        notify('warning', 'Chưa đăng nhập', 'Bạn cần đăng nhập/đăng ký để đặt hàng. Đang chuyển hướng...');
        setTimeout(() => { window.location.href = 'auth.html'; }, 1200);
        return;
      }
      window.location.href = 'checkout.html';
    });
  }

  render();

  window.addEventListener('storage', e => {
    if (e.key === CART_KEY) render();
  });
})();