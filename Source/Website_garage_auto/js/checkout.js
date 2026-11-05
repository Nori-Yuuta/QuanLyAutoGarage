const CART_KEY = 'autogarage_cart';
const CURRENT_KEY = 'autogarage_current';
const ORDER_KEY = 'autogarage_orders';

function loadCart() {
  try { return JSON.parse(localStorage.getItem(CART_KEY)) || []; }
  catch { return []; }
}
function saveCart(c) { localStorage.setItem(CART_KEY, JSON.stringify(c)); }
function formatPrice(n) { return n.toLocaleString('vi-VN') + ' ₫'; }

function toast(msg, type = 'success') {
  const el = document.getElementById('toast');
  el.textContent = msg;
  el.className = `toast show ${type}`;
  setTimeout(() => el.className = 'toast', 2200);
}

const current = JSON.parse(localStorage.getItem(CURRENT_KEY) || 'null');
if (!current) { window.location.href = 'auth.html'; }

const cart = loadCart();
if (!cart.length) { window.location.href = 'cart.html'; }

document.querySelector('[name="name"]').value = current.name || '';
document.querySelector('[name="phone"]').value = current.phone || '';
document.querySelector('[name="email"]').value = current.email || '';

document.getElementById('checkout-items').innerHTML = cart.map(i => `
  <div class="checkout-item">
    <span class="checkout-item-name">${i.name}</span>
    <span class="checkout-item-qty">×${i.qty}</span>
    <span class="checkout-item-price">${formatPrice(i.price * i.qty)}</span>
  </div>
`).join('');

const COUPONS = {
  'SALE30': { type: 'percent', value: 30, max: 500000, desc: 'Giảm 30% tối đa 500.000 ₫' },
  'WELCOME10': { type: 'percent', value: 10, max: 200000, desc: 'Giảm 10% cho khách mới' },
  'SHIPFREE': { type: 'shipping', value: 100, desc: 'Miễn phí vận chuyển' }
};

let activeCoupon = null;

function calcTotals() {
  const subtotal = cart.reduce((s, i) => s + i.price * i.qty, 0);
  let shipping = subtotal >= 500000 ? 0 : 30000;
  let discount = 0;

  const coupon = COUPONS[activeCoupon];
  if (coupon) {
    if (coupon.type === 'percent') {
      discount = Math.min(subtotal * coupon.value / 100, coupon.max);
    } else if (coupon.type === 'shipping') {
      shipping = 0;
    }
  }

  const total = Math.max(0, subtotal + shipping - discount);

  document.getElementById('co-subtotal').textContent = formatPrice(subtotal);
  document.getElementById('co-shipping').textContent = shipping === 0 ? 'Miễn phí' : formatPrice(shipping);

  const discRow = document.getElementById('discount-row');
  if (discount > 0) {
    discRow.style.display = 'flex';
    document.getElementById('co-discount').textContent = '-' + formatPrice(discount);
  } else {
    discRow.style.display = 'none';
  }

  document.getElementById('co-total').textContent = formatPrice(total);

  return { subtotal, shipping, discount, total };
}

document.getElementById('apply-coupon-btn').addEventListener('click', () => {
  const input = document.getElementById('coupon-input');
  const msg = document.getElementById('coupon-msg');
  const code = input.value.trim().toUpperCase();

  if (!code) {
    msg.textContent = 'Vui lòng nhập mã.';
    msg.className = 'coupon-msg err';
    return;
  }

  if (COUPONS[code]) {
    activeCoupon = code;
    msg.textContent = `✓ ${COUPONS[code].desc}`;
    msg.className = 'coupon-msg ok';
    calcTotals();
  } else {
    activeCoupon = null;
    msg.textContent = '✗ Mã không hợp lệ hoặc đã hết hạn.';
    msg.className = 'coupon-msg err';
    calcTotals();
  }
});

calcTotals();

document.getElementById('checkout-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const totals = calcTotals();

  const order = {
    id: 'ORD' + Date.now(),
    user: current.email,
    recipient: {
      name: f.name.value.trim(),
      phone: f.phone.value.trim(),
      email: f.email.value.trim(),
      province: f.province.value,
      district: f.district.value.trim(),
      address: f.address.value.trim(),
      note: f.note.value.trim()
    },
    payment: f.payment.value,
    items: cart,
    coupon: activeCoupon,
    totals,
    createdAt: new Date().toISOString()
  };

  const orders = JSON.parse(localStorage.getItem(ORDER_KEY) || '[]');
  orders.push(order);
  localStorage.setItem(ORDER_KEY, JSON.stringify(orders));

saveCart([]);

Toast.success(
  'Đặt hàng thành công!',
  `Mã đơn: ${order.id} — Tổng: ${formatPrice(totals.total)}`,
  5000
);

setTimeout(() => {
  window.location.href = 'index.html';
}, 2000);
});