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

// Helpers
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

// ==========================================
// TỰ ĐỘNG KHỞI TẠO TÀI KHOẢN ADMIN (CHỈ CHẠY 1 LẦN)
// ==========================================
function initAdminAccount() {
  const users = loadUsers();
  // Kiểm tra xem đã có tài khoản admin nào chưa
  if (!users.some(u => u.email === 'admin@autogarage.vn')) {
    users.push({
      name: 'Quản Trị Viên',
      email: 'admin@autogarage.vn',
      phone: '0999999999',
      password: 'admin123',
      role: 'admin',
      createdAt: new Date().toISOString()
    });
    saveUsers(users);
    console.log('✅ Đã tự động tạo tài khoản Admin: admin@autogarage.vn / admin123');
  }
}
// Gọi hàm khởi tạo ngay khi tải trang
initAdminAccount();


// Đăng ký (Mặc định role là 'customer')
document.getElementById('register-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const data = {
    name: f.name.value.trim(),
    email: f.email.value.trim().toLowerCase(),
    phone: f.phone.value.trim(),
    password: f.password.value,
    confirm: f.confirm.value,
    role: 'customer', // Mặc định là khách hàng
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

// Đăng nhập (Có phân quyền)
document.getElementById('login-form').addEventListener('submit', e => {
  e.preventDefault();
  const f = e.target;
  const email = f.email.value.trim().toLowerCase();
  const password = f.password.value;

  const users = loadUsers();
  // Tìm user khớp email & pass
  const user = users.find(u => u.email === email && u.password === password);

  if (!user) {
    Toast.error('Đăng nhập thất bại', 'Email hoặc mật khẩu không đúng');
    return;
  }

  // Lưu user vào phiên hiện tại
  setCurrent(user);
  
  // Log kiểm tra (Mở F12 xem có in ra không)
  console.log('Đăng nhập thành công:', user);
  console.log('Role của user này là:', user.role);

  Toast.success('Chào mừng trở lại!', `Xin chào ${user.name}`);
  
  // CHUYỂN HƯỚNG DỰA VÀO ROLE
  setTimeout(() => { 
    if (user.role === 'admin') {
      console.log('Chuyển hướng tới admin.html...');
      window.location.href = 'admin.html';
    } else {
      console.log('Chuyển hướng tới profile.html...');
      window.location.href = 'profile.html';
    }
  }, 1000); // Đợi 1 giây để Toast kịp hiện
});