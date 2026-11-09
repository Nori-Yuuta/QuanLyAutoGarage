const USER_KEY = 'autogarage_users';
const CURRENT_KEY = 'autogarage_current';

function getUser() {
  try { return JSON.parse(localStorage.getItem(CURRENT_KEY)); }
  catch { return null; }
}
function loadUsers() {
  try { return JSON.parse(localStorage.getItem(USER_KEY)) || []; }
  catch { return []; }
}
function saveUsers(u) { localStorage.setItem(USER_KEY, JSON.stringify(u)); }
function setCurrent(u) { localStorage.setItem(CURRENT_KEY, JSON.stringify(u)); }

// Bảo vệ trang
const currentUser = getUser();
if (!currentUser) {
  window.location.href = 'auth.html';
}

// THÊM ĐOẠN NÀY: Nếu là Admin thì đá về Dashboard (Chỉ cần 1 lần)
if (currentUser.role === 'admin') {
  window.location.href = 'admin.html';
  return;
}

// Render thông tin
function renderInfo() {
  const u = getUser();
  document.getElementById('avatar-initial').textContent =
    (u.name || '?').trim().charAt(0).toUpperCase();
  document.getElementById('sidebar-name').textContent = u.name || 'Khách hàng';
  document.getElementById('sidebar-email').textContent = u.email || '—';

  const infoForm = document.getElementById('info-form');
  infoForm.name.value = u.name || '';
  infoForm.phone.value = u.phone || '';
  infoForm.email.value = u.email || '';
}

renderInfo();

// Tabs panel
document.querySelectorAll('.profile-nav-item[data-panel]').forEach(a => {
  a.addEventListener('click', e => {
    e.preventDefault();
    document.querySelectorAll('.profile-nav-item').forEach(x => x.classList.remove('active'));
    a.classList.add('active');
    document.querySelectorAll('.panel').forEach(p => p.classList.remove('active'));
    document.getElementById('panel-' + a.dataset.panel).classList.add('active');
  });
});

// Cập nhật thông tin
document.getElementById('info-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const u = getUser();
  const users = loadUsers();

  const newEmail = f.email.value.trim().toLowerCase();
  if (users.some(x => x.email === newEmail && x.email !== u.email)) {
    Toast.warning('Email đã tồn tại', 'Email này đã được dùng bởi tài khoản khác');
    return;
  }

  const idx = users.findIndex(x => x.email === u.email);
  if (idx === -1) {
    Toast.error('Không tìm thấy tài khoản', 'Vui lòng đăng nhập lại');
    return;
  }

  users[idx] = {
    ...users[idx],
    name: f.name.value.trim(),
    phone: f.phone.value.trim(),
    email: newEmail
  };

  saveUsers(users);
  setCurrent(users[idx]);
  renderInfo();
  Toast.success('Đã lưu thay đổi', 'Thông tin cá nhân đã được cập nhật');
});

// Đổi mật khẩu
document.getElementById('password-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const u = getUser();
  const users = loadUsers();
  const idx = users.findIndex(x => x.email === u.email);

  if (idx === -1) {
    Toast.error('Không tìm thấy tài khoản', 'Vui lòng đăng nhập lại');
    return;
  }

  if (users[idx].password !== f.oldPassword.value) {
    Toast.error('Sai mật khẩu', 'Mật khẩu hiện tại không đúng');
    return;
  }

  if (f.newPassword.value !== f.confirmPassword.value) {
    Toast.error('Không khớp', 'Mật khẩu xác nhận không khớp với mật khẩu mới');
    return;
  }

  users[idx].password = f.newPassword.value;
  saveUsers(users);
  setCurrent(users[idx]);
  f.reset();
  Toast.success('Đổi mật khẩu thành công', 'Lần đăng nhập sau hãy dùng mật khẩu mới');
});

// Đăng xuất
document.getElementById('logout-btn').addEventListener('click', () => {
  localStorage.removeItem(CURRENT_KEY);
  Toast.info('Đã đăng xuất', 'Hẹn gặp bạn lần sau!');
  setTimeout(() => { window.location.href = 'index.html'; }, 800);
});