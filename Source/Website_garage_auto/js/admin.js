/* ====================================================
   ADMIN DASHBOARD LOGIC (BẢN NÂNG CẤP)
   ==================================================== */

// --- 1. KIỂM TRA QUYỀN & THÔNG TIN ---
let currentUser = JSON.parse(localStorage.getItem('autogarage_current'));
if (!currentUser) {
    window.location.href = 'auth.html';
} else if (currentUser.role !== 'admin') {
    alert('Bạn không có quyền truy cập trang này!');
    window.location.href = 'index.html';
}

// Render thông tin lên Header
function renderHeaderInfo() {
  document.getElementById('header-admin-name').textContent = currentUser.name;
  document.getElementById('header-admin-email').textContent = currentUser.email;
  document.getElementById('header-admin-avatar').textContent = currentUser.name.charAt(0).toUpperCase();
}
renderHeaderInfo();

// Click vào profile góc phải -> Về Dashboard
document.getElementById('admin-profile-area').addEventListener('click', () => {
  document.querySelector('.admin-nav-item[data-tab="dashboard"]').click();
});

// Đăng xuất
document.getElementById('admin-logout-btn').addEventListener('click', () => {
  localStorage.removeItem('autogarage_current');
  window.location.href = 'index.html';
});

// --- 2. KHO DỮ LIỆU ---
if (!localStorage.getItem('db_cars')) localStorage.setItem('db_cars', JSON.stringify(carsData || []));
if (!localStorage.getItem('db_accessories')) localStorage.setItem('db_accessories', JSON.stringify(accessoriesData || []));
if (!localStorage.getItem('db_notifications')) localStorage.setItem('db_notifications', JSON.stringify([]));
if (!localStorage.getItem('db_events')) localStorage.setItem('db_events', JSON.stringify([]));

function getDB(key) { return JSON.parse(localStorage.getItem(key)) || []; }
function setDB(key, data) { localStorage.setItem(key, JSON.stringify(data)); }

// --- 3. CHUYỂN TAB ---
document.querySelectorAll('.admin-nav-item').forEach(item => {
  item.addEventListener('click', (e) => {
    e.preventDefault();
    document.querySelectorAll('.admin-nav-item').forEach(nav => nav.classList.remove('active'));
    document.querySelectorAll('.admin-tab').forEach(tab => tab.classList.remove('active'));
    
    item.classList.add('active');
    const tabId = 'tab-' + item.dataset.tab;
    if(document.getElementById(tabId)) {
        document.getElementById(tabId).classList.add('active');
        document.getElementById('page-heading').textContent = item.textContent.trim();
    }

    if (item.dataset.tab === 'dashboard') renderDashboard();
    if (item.dataset.tab === 'cars') renderTable('cars');
    if (item.dataset.tab === 'accessories') renderTable('accessories');
    if (item.dataset.tab === 'notifications') renderTable('notifications');
    if (item.dataset.tab === 'events') renderTable('events');
    
    // Nếu mở tab Tài khoản thì load dữ liệu vào form
    if (item.dataset.tab === 'account') {
        document.getElementById('acc-name').value = currentUser.name;
        document.getElementById('acc-email').value = currentUser.email;
        document.getElementById('acc-phone').value = currentUser.phone || '';
        document.getElementById('acc-old-pass').value = '';
        document.getElementById('acc-new-pass').value = '';
    }
  });
});

// --- 4. XỬ LÝ FORM THÔNG TIN TÀI KHOẢN ---
document.getElementById('admin-account-form').addEventListener('submit', (e) => {
  e.preventDefault();
  const newName = document.getElementById('acc-name').value.trim();
  const newEmail = document.getElementById('acc-email').value.trim().toLowerCase();
  const newPhone = document.getElementById('acc-phone').value.trim();
  const oldPass = document.getElementById('acc-old-pass').value;
  const newPass = document.getElementById('acc-new-pass').value;

  let users = JSON.parse(localStorage.getItem('autogarage_users')) || [];
  
  // Kiểm tra email trùng
  if (users.some(u => u.email === newEmail && u.email !== currentUser.email)) {
      Toast.error('Lỗi', 'Email này đã được sử dụng bởi tài khoản khác!');
      return;
  }

  const idx = users.findIndex(u => u.email === currentUser.email);
  if (idx === -1) {
      Toast.error('Lỗi', 'Không tìm thấy tài khoản trong hệ thống!');
      return;
  }

  // Xử lý đổi mật khẩu
  if (newPass) {
      if (oldPass !== users[idx].password) {
          Toast.error('Lỗi', 'Mật khẩu hiện tại không đúng!');
          return;
      }
      users[idx].password = newPass;
  }

  // Cập nhật thông tin
  users[idx].name = newName;
  users[idx].email = newEmail;
  users[idx].phone = newPhone;

  // Lưu lại
  localStorage.setItem('autogarage_users', JSON.stringify(users));
  localStorage.setItem('autogarage_current', JSON.stringify(users[idx]));
  currentUser = users[idx]; // Cập nhật biến toàn cục

  renderHeaderInfo();
  Toast.success('Thành công', 'Đã cập nhật thông tin tài khoản!');
  document.getElementById('acc-old-pass').value = '';
  document.getElementById('acc-new-pass').value = '';
});


// --- 5. RENDER DASHBOARD ---
function renderDashboard() {
  const orders = getDB('autogarage_orders') || [];
  const cars = getDB('db_cars');
  const accs = getDB('db_accessories');

  let totalRevenue = 0;
  orders.forEach(o => { if (o.totals && o.totals.total) totalRevenue += o.totals.total; });

  document.getElementById('stat-revenue').textContent = totalRevenue.toLocaleString('vi-VN') + ' ₫';
  document.getElementById('stat-orders').textContent = orders.length;
  document.getElementById('stat-cars').textContent = cars.length;
  document.getElementById('stat-accs').textContent = accs.length;

  const tbody = document.getElementById('recent-orders-table');
  if (orders.length === 0) {
    tbody.innerHTML = '<tr><td colspan="5" style="text-align:center;color:#888;">Chưa có đơn hàng nào.</td></tr>';
    return;
  }
  tbody.innerHTML = [...orders].reverse().slice(0, 5).map(ord => `
    <tr>
      <td><b>${ord.id}</b></td>
      <td>${ord.recipient.name}</td>
      <td>${ord.recipient.phone}</td>
      <td style="color:#e63946;font-weight:bold;">${ord.totals.total.toLocaleString('vi-VN')} ₫</td>
      <td>${new Date(ord.createdAt).toLocaleDateString('vi-VN')}</td>
    </tr>
  `).join('');
}

// --- 6. RENDER BẢNG ---
function renderTable(type) {
  const tbody = document.getElementById(`${type}-table-body`);
  if (!tbody) return;
  const data = getDB(`db_${type}`);
  
  tbody.innerHTML = data.map(item => {
    let row = `<td>#${item.id}</td>`;
    
    let imgDisplay = item.img && item.img.startsWith('data:image') 
        ? `<img src="${item.img}" style="width:40px;height:40px;object-fit:cover;border-radius:4px;">` 
        : `<span style="font-size:24px">${item.img || '📦'}</span>`;
    
    row += `<td>${imgDisplay}</td>`;

    if (type === 'cars') {
      row += `<td><b>${item.name}</b></td><td>${item.brand}</td><td style="color:#e63946">${item.price}</td>`;
    } else if (type === 'accessories') {
      row += `<td><b>${item.name}</b></td><td>${item.category}</td><td style="color:#e63946">${item.price}</td>`;
    } else if (type === 'notifications') {
      row += `<td><b>${item.title}</b></td><td>${item.content}</td><td>${item.date}</td>`;
    } else if (type === 'events') {
      row += `<td><b>${item.name}</b></td><td><span style="background:#ffcc00;padding:2px 8px;border-radius:4px;font-weight:bold">${item.code}</span></td><td>${item.endDate}</td><td>${item.status}</td>`;
    }
    
    row += `<td>
        <button class="action-btn edit" onclick="openModal('${type}', ${item.id})">Sửa</button>
        <button class="action-btn delete" onclick="deleteItem('${type}', ${item.id})">Xóa</button>
      </td>`;
    return `<tr>${row}</tr>`;
  }).join('');
}

// --- 7. XỬ LÝ MODAL (CẤU HÌNH ĐỘNG) ---
const modal = document.getElementById('admin-modal');
const modalTitle = document.getElementById('modal-title');
const modalForm = document.getElementById('modal-form');
const modalFields = document.getElementById('modal-form-fields');
const modalEditId = document.getElementById('modal-edit-id');
let currentType = '';

// Tạo danh sách năm cho dropdown
const currentYear = new Date().getFullYear();
let yearOptions = [];
for (let y = currentYear; y >= currentYear - 20; y--) yearOptions.push(y);

// Cấu hình các trường nhập liệu
const fieldConfigs = {
  cars: [
    { name: 'name', label: 'Tên Xe', type: 'text', required: true },
    { name: 'brand', label: 'Hãng Xe', type: 'select', options: ['Toyota', 'Honda', 'Ford', 'Mazda', 'BMW', 'Mercedes-Benz'], required: true },
    { name: 'price', label: 'Giá (VNĐ - Chỉ nhập số)', type: 'text', required: true, isPrice: true },
    { name: 'img_url', label: 'Hình Ảnh Xe (Upload)', type: 'file', required: false },
    { name: 'year', label: 'Năm SX', type: 'select', options: yearOptions, required: true }, // Đã chuyển thành Select
    { name: 'engine', label: 'Động cơ', type: 'text' },
    { name: 'fuel', label: 'Nhiên liệu', type: 'text' },
    { name: 'seats', label: 'Số chỗ', type: 'number' },
    { name: 'desc', label: 'Mô tả', type: 'textarea' }
  ],
  accessories: [
    { name: 'name', label: 'Tên Phụ Kiện', type: 'text', required: true },
    { name: 'category', label: 'Danh Mục', type: 'select', options: ['Nội thất', 'Ngoại thất', 'An toàn', 'Vận hành'], required: true },
    { name: 'price', label: 'Giá (VNĐ - Chỉ nhập số)', type: 'text', required: true, isPrice: true },
    { name: 'img_url', label: 'Hình Ảnh (Upload)', type: 'file', required: false },
    { name: 'material', label: 'Chất liệu (không bắt buộc)', type: 'text' }, // Đã thêm chữ
    { name: 'warranty', label: 'Thời gian Bảo hành', type: 'warranty_range', required: true }, // Trường đặc biệt
    { name: 'desc', label: 'Mô tả', type: 'textarea' }
  ],
  notifications: [
    { name: 'title', label: 'Tiêu Đề', type: 'text', required: true },
    { name: 'content', label: 'Nội Dung', type: 'textarea', required: true },
    { name: 'date', label: 'Ngày (YYYY-MM-DD)', type: 'date', required: true }
  ],
  events: [
    { name: 'name', label: 'Tên Sự Kiện', type: 'text', required: true },
    { name: 'code', label: 'Mã Giảm Giá', type: 'text', required: true },
    { name: 'endDate', label: 'Ngày Kết Thúc', type: 'date', required: true },
    { name: 'status', label: 'Trạng Thái', type: 'select', options: ['Đang diễn ra', 'Sắp diễn ra', 'Đã kết thúc'] }
  ]
};

window.openModal = function(type, id = null) {
  currentType = type;
  modalEditId.value = id || '';
  modalTitle.textContent = id ? 'Chỉnh Sửa' : 'Thêm Mới';
  
  document.getElementById('excel-upload-group').style.display = id ? 'none' : 'block';
  document.getElementById('excel-file').value = '';

  // Render form fields dựa trên Config
  modalFields.innerHTML = fieldConfigs[type].map(f => {
    let inputHtml = '';
    
    if (f.type === 'file') {
      inputHtml = `
        <input type="file" name="${f.name}" accept="image/*" onchange="previewImage(event, '${f.name}-preview')">
        <div style="margin-top:10px;">
            <img id="${f.name}-preview" src="" style="max-height:100px; display:none; border-radius:6px; border:1px solid #ddd;">
            <input type="hidden" name="${f.name}_base64" id="${f.name}_base64">
        </div>
      `;
    } 
    else if (f.type === 'textarea') {
      inputHtml = `<textarea name="${f.name}" rows="3" ${f.required ? 'required' : ''}></textarea>`;
    } 
    else if (f.type === 'select') {
      inputHtml = `<select name="${f.name}" ${f.required ? 'required' : ''}>${f.options.map(o => `<option value="${o}">${o}</option>`).join('')}</select>`;
    } 
    // Xử lý riêng trường Bảo hành (Chọn từ ngày - đến ngày)
    else if (f.type === 'warranty_range') {
       inputHtml = `
        <div style="display:flex; gap:10px; align-items:center;">
            <input type="month" name="warranty_from" id="warranty_from" style="width: 50%;" required>
            <span>đến</span>
            <input type="month" name="warranty_to" id="warranty_to" style="width: 50%;" required>
        </div>
       `;
    }
    // Xử lý trường giá (chỉ cho nhập số)
    else if (f.isPrice) {
      inputHtml = `<input type="text" name="${f.name}" oninput="this.value = this.value.replace(/[^0-9]/g, '')" placeholder="VD: 15000000" ${f.required ? 'required' : ''}>`;
    }
    else {
      inputHtml = `<input type="${f.type}" name="${f.name}" ${f.required ? 'required' : ''}>`;
    }
    return `<div class="form-group"><label>${f.label}</label>${inputHtml}</div>`;
  }).join('');

  // Điền dữ liệu cũ nếu là Sửa
  if (id) {
    const data = getDB(`db_${type}`).find(item => item.id === id);
    Object.keys(data).forEach(key => {
      const input = modalForm.querySelector(`[name="${key}"]`);
      if (input) input.value = data[key];
    });

    if (data.img) {
      const preview = document.getElementById('img_url-preview');
      const hiddenInput = document.getElementById('img_url_base64');
      if (preview && data.img.startsWith('data:image')) { 
          preview.src = data.img; 
          preview.style.display = 'block'; 
      }
      if (hiddenInput) hiddenInput.value = data.img;
    }
    
    // Trường đặc biệt: Bảo hành (Tách chuỗi "Từ A đến B" ngược lại vào 2 ô)
    if (type === 'accessories' && data.warranty) {
        const parts = data.warranty.split(' đến ');
        if(parts.length === 2) {
            document.getElementById('warranty_from').value = parts[0];
            document.getElementById('warranty_to').value = parts[1];
        }
    }
  }

  modal.classList.add('active');
};

window.previewImage = function(event, previewId) {
  const file = event.target.files[0];
  if (file) {
    const reader = new FileReader();
    reader.onload = function(e) {
      const preview = document.getElementById(previewId);
      preview.src = e.target.result;
      preview.style.display = 'block';
      document.getElementById(previewId.replace('-preview', '_base64')).value = e.target.result;
    };
    reader.readAsDataURL(file);
  }
};

window.closeModal = function() {
  modal.classList.remove('active');
  modalForm.reset();
};

// Xử lý khi Lưu Form (Thêm / Sửa)
modalForm.addEventListener('submit', (e) => {
  e.preventDefault();
  const formData = new FormData(modalForm);
  let dataObj = Object.fromEntries(formData.entries());
  const id = modalEditId.value ? Number(modalEditId.value) : null;
  
  // --- XỬ LÝ ĐẶC BIỆT TỪNG BẢNG ---
  
  // 1. Xử lý Giá (Định dạng lại thành chuỗi có chữ VNĐ)
  if (currentType === 'cars' || currentType === 'accessories') {
      if(!dataObj.price) {
          Toast.error('Lỗi', 'Vui lòng nhập giá!');
          return;
      }
      // Định dạng lại: 15000000 -> 15.000.000 ₫
      dataObj.price = Number(dataObj.price).toLocaleString('vi-VN') + ' ₫';
  }

  // 2. Xử lý Hình ảnh
  const base64Img = document.getElementById('img_url_base64')?.value;
  const textImg = document.querySelector('[name="img_url"]')?.value; 
  if (currentType === 'cars' || currentType === 'accessories') {
      dataObj.img = base64Img || textImg || '📦';
      delete dataObj.img_url;
      delete dataObj.img_url_base64;
  }

  // 3. Xử lý Bảo hành (Ghép 2 ô date thành 1 chuỗi)
  if (currentType === 'accessories' && dataObj.warranty_from && dataObj.warranty_to) {
      dataObj.warranty = `${dataObj.warranty_from} đến ${dataObj.warranty_to}`;
      delete dataObj.warranty_from;
      delete dataObj.warranty_to;
  }

  let db = getDB(`db_${currentType}`);

  if (id) {
    const index = db.findIndex(item => item.id === id);
    db[index] = { ...db[index], ...dataObj, id: id };
    Toast.success('Thành công', 'Đã cập nhật dữ liệu.');
  } else {
    const newId = db.length > 0 ? Math.max(...db.map(i => i.id)) + 1 : 1;
    db.push({ ...dataObj, id: newId });
    Toast.success('Thành công', 'Đã thêm dữ liệu mới.');
  }

  setDB(`db_${currentType}`, db);
  closeModal();
  renderTable(currentType);
});

window.deleteItem = function(type, id) {
  if (confirm('Bạn có chắc chắn muốn xóa mục này không?')) {
    let db = getDB(`db_${type}`).filter(item => item.id !== id);
    setDB(`db_${type}`, db);
    Toast.info('Đã xóa', 'Dữ liệu đã được xóa khỏi hệ thống.');
    renderTable(type);
    if(type === 'cars' || type === 'accessories') renderDashboard();
  }
};

// Đọc Excel
document.getElementById('excel-file').addEventListener('change', function(e) {
  const file = e.target.files[0];
  if (!file) return;

  const reader = new FileReader();
  reader.onload = function(evt) {
    try {
      const data = new Uint8Array(evt.target.result);
      const workbook = XLSX.read(data, { type: 'array' });
      const firstSheet = workbook.Sheets[workbook.SheetNames[0]];
      const rows = XLSX.utils.sheet_to_json(firstSheet);
      
      if (rows.length === 0) {
        Toast.error('Lỗi', 'File Excel trống!');
        return;
      }

      const firstRow = rows[0];
      Object.keys(firstRow).forEach(key => {
        const input = modalForm.querySelector(`[name="${key}"]`);
        if (input) {
          input.value = firstRow[key];
        }
      });

      Toast.success('Đọc Excel thành công', 'Thông tin đã được điền tự động vào form.');
    } catch (err) {
      console.error(err);
      Toast.error('Lỗi đọc file', 'Vui lòng kiểm tra lại định dạng Excel.');
    }
  };
  reader.readAsArrayBuffer(file);
});

// Chatbox Logic
const chatForm = document.getElementById('chat-form');
if(chatForm) {
    chatForm.addEventListener('submit', (e) => {
      e.preventDefault();
      const chatInput = document.getElementById('chat-input');
      const chatMessages = document.getElementById('chat-messages');
      const text = chatInput.value.trim();
      if (!text) return;
      
      const sentMsg = document.createElement('div');
      sentMsg.className = 'msg sent';
      sentMsg.textContent = text;
      chatMessages.appendChild(sentMsg);
      chatInput.value = '';
      chatMessages.scrollTop = chatMessages.scrollHeight;
    });
}

// Khởi tạo
renderDashboard();