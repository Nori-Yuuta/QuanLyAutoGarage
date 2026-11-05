/* ============================================
   AUTOGARAGE TOAST — Custom Notification Library
   Dùng: Toast.success('Tiêu đề', 'Nội dung')
         Toast.error('Lỗi', 'Sai mật khẩu')
         Toast.warning(...), Toast.info(...), Toast.cart(...)
   ============================================ */

(function (global) {
  // Tạo container 1 lần duy nhất
  let container = null;

  function getContainer() {
    if (container && document.body.contains(container)) return container;
    container = document.createElement('div');
    container.className = 'toast-container';
    document.body.appendChild(container);
    return container;
  }

  // SVG icons
  const ICONS = {
    success: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>',
    error:   '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>',
    warning: '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>',
    info:    '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>',
    cart:    '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle><path d="M1 1h4l2.6 13.4a2 2 0 0 0 2 1.6h9.7a2 2 0 0 0 2-1.6L23 6H6"></path></svg>'
  };

  const CLOSE_ICON = '<svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>';

  const DEFAULT_TITLES = {
    success: 'Thành công',
    error:   'Có lỗi xảy ra',
    warning: 'Lưu ý',
    info:    'Thông báo',
    cart:    'Giỏ hàng'
  };

  /**
   * Hiển thị toast
   * @param {string} type  - 'success' | 'error' | 'warning' | 'info' | 'cart'
   * @param {string} title - tiêu đề (nếu chỉ truyền 1 tham số thì đây là message)
   * @param {string} message - nội dung
   * @param {number} duration - thời gian hiển thị (ms), mặc định 3500
   */
  function show(type, title, message, duration) {
    // Cho phép gọi Toast.success('Nội dung duy nhất')
    if (message === undefined) {
      message = title;
      title = DEFAULT_TITLES[type] || 'Thông báo';
    }

    duration = duration || 3500;
    const c = getContainer();

    const item = document.createElement('div');
    item.className = `toast-item ${type}`;
    item.innerHTML = `
      <div class="toast-icon">${ICONS[type] || ICONS.info}</div>
      <div class="toast-body">
        <div class="toast-title">${escapeHtml(title)}</div>
        <div class="toast-message">${escapeHtml(message)}</div>
      </div>
      <button class="toast-close" aria-label="Đóng">${CLOSE_ICON}</button>
      <div class="toast-progress" style="animation-duration:${duration}ms"></div>
    `;

    c.appendChild(item);

    // Hiệu ứng xuất hiện
    requestAnimationFrame(() => {
      requestAnimationFrame(() => item.classList.add('show'));
    });

    // Auto dismiss
    let timer = setTimeout(() => dismiss(item), duration);

    // Đóng thủ công
    item.querySelector('.toast-close').addEventListener('click', () => {
      clearTimeout(timer);
      dismiss(item);
    });

    // Hover tạm dừng progress
    item.addEventListener('mouseenter', () => {
      clearTimeout(timer);
      const bar = item.querySelector('.toast-progress');
      if (bar) bar.style.animationPlayState = 'paused';
    });

    item.addEventListener('mouseleave', () => {
      const bar = item.querySelector('.toast-progress');
      if (bar) bar.style.animationPlayState = 'running';
      timer = setTimeout(() => dismiss(item), 1500);
    });
  }

  function dismiss(item) {
    if (!item || !item.parentNode) return;
    item.classList.remove('show');
    item.classList.add('hide');
    setTimeout(() => {
      if (item.parentNode) item.parentNode.removeChild(item);
    }, 400);
  }

  function escapeHtml(str) {
    const d = document.createElement('div');
    d.textContent = str == null ? '' : String(str);
    return d.innerHTML;
  }

  // Public API
  const Toast = {
    success: (title, msg, dur) => show('success', title, msg, dur),
    error:   (title, msg, dur) => show('error', title, msg, dur),
    warning: (title, msg, dur) => show('warning', title, msg, dur),
    info:    (title, msg, dur) => show('info', title, msg, dur),
    cart:    (title, msg, dur) => show('cart', title, msg, dur),
    show
  };

  global.Toast = Toast;
})(window);