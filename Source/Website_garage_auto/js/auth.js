// Xử lý tab login/register
document.querySelectorAll('.auth-tab').forEach(tab => {
  tab.addEventListener('click', () => {
    document.querySelectorAll('.auth-tab').forEach(t => t.classList.remove('active'));
    tab.classList.add('active');
    showForm(tab.dataset.tab);
  });
});

document.querySelectorAll('[data-switch]').forEach(a => {
  a.addEventListener('click', e => {
    e.preventDefault();
    const target = a.dataset.switch;
    document.querySelectorAll('.auth-tab').forEach(t => {
      t.classList.toggle('active', t.dataset.tab === target);
    });
    showForm(target);
  });
});

function showForm(name) {
  document.getElementById('login-form').classList.toggle('hidden', name !== 'login');
  document.getElementById('register-form').classList.toggle('hidden', name !== 'register');
}

// Helpers user store
const USER_KEY = 'autogarage_users';
const CURRENT_KEY = 'autogarage_current';

function loadUsers() {
  try { return JSON.parse(localStorage.getItem(USER_KEY)) || []; }
  catch { return []; }
}
function saveUsers(users) {
  localStorage.setItem(USER_KEY, JSON.stringify(users));
}
function setCurrent(user) {
  localStorage.setItem(CURRENT_KEY, JSON.stringify(user));
}

// Đăng ký
document.getElementById('register-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const data = {
    name: f.name.value.trim(),
    email: f.email.value.trim().toLowerCase(),
    phone: f.phone.value.trim(),
    password: f.password.value,
    confirm: f.confirm.value,
    createdAt: new Date().toISOString()
  };

  if (data.password !== data.confirm) {
    Toast.error('Mật khẩu không khớp', 'Vui lòng kiểm tra lại xác nhận mật khẩu');
    return;
  }

  const users = loadUsers();
  if (users.some(u => u.email === data.email)) {
    Toast.warning('Email đã được đăng ký', 'Vui lòng dùng email khác hoặc đăng nhập');
    return;
  }

  delete data.confirm;
  users.push(data);
  saveUsers(users);

  Toast.success('Đăng ký thành công!', 'Đang chuyển sang đăng nhập...');

  setTimeout(() => {
    document.querySelector('[data-tab="login"]').click();
    document.getElementById('login-form').email.value = data.email;
    document.getElementById('login-form').password.value = '';
  }, 1200);
});

// Đăng nhập
document.getElementById('login-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const email = f.email.value.trim().toLowerCase();
  const password = f.password.value;

  const users = loadUsers();
  const user = users.find(u => u.email === email && u.password === password);

  if (!user) {
    Toast.error('Đăng nhập thất bại', 'Email hoặc mật khẩu không đúng');
    return;
  }

  setCurrent(user);
  Toast.success('Chào mừng trở lại!', `Xin chào ${user.name}`);
  setTimeout(() => { window.location.href = 'profile.html'; }, 900);
});