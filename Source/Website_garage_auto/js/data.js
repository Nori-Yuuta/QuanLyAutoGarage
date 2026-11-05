// Dữ liệu xe
const carsData = [
  {
    id: 1,
    name: 'Toyota Camry 2024',
    brand: 'Toyota',
    price: '1.150.000.000 ₫',
    img: '🚗',
    year: 2024,
    engine: '2.5L Hybrid',
    fuel: 'Xăng / Hybrid',
    seats: 5,
    transmission: 'Tự động 8 cấp',
    origin: 'Nhật Bản',
    desc: 'Sedan hạng D sang trọng, tiết kiệm nhiên liệu, an toàn 5 sao.'
  },
  {
    id: 2,
    name: 'Honda CR-V 2024',
    brand: 'Honda',
    price: '1.050.000.000 ₫',
    img: '🚙',
    year: 2024,
    engine: '1.5L Turbo',
    fuel: 'Xăng',
    seats: 7,
    transmission: 'CVT',
    origin: 'Nhật Bản',
    desc: 'SUV 7 chỗ đa dụng, phù hợp gia đình, khoang hành lý rộng.'
  },
  {
    id: 3,
    name: 'Ford Ranger Wildtrak',
    brand: 'Ford',
    price: '920.000.000 ₫',
    img: '🛻',
    year: 2024,
    engine: '2.0L Bi-Turbo',
    fuel: 'Dầu',
    seats: 5,
    transmission: 'Tự động 10 cấp',
    origin: 'Mỹ',
    desc: 'Bán tải mạnh mẽ, khả năng off-road tốt, nội thất tiện nghi.'
  },
  {
    id: 4,
    name: 'Mazda CX-5 2024',
    brand: 'Mazda',
    price: '880.000.000 ₫',
    img: '🚘',
    year: 2024,
    engine: '2.0L SkyActiv',
    fuel: 'Xăng',
    seats: 5,
    transmission: 'Tự động 6 cấp',
    origin: 'Nhật Bản',
    desc: 'Crossover thiết kế KODO đẹp mắt, vận hành êm ái.'
  },
  {
    id: 5,
    name: 'BMW 320i 2024',
    brand: 'BMW',
    price: '1.550.000.000 ₫',
    img: '🏎️',
    year: 2024,
    engine: '2.0L TwinPower Turbo',
    fuel: 'Xăng',
    seats: 5,
    transmission: 'Tự động 8 cấp',
    origin: 'Đức',
    desc: 'Sedan thể thao hạng sang, cảm giác lái phấn khích.'
  },
  {
    id: 6,
    name: 'Mercedes C200 2024',
    brand: 'Mercedes-Benz',
    price: '1.690.000.000 ₫',
    img: '🚗',
    year: 2024,
    engine: '1.5L Turbo + EQ Boost',
    fuel: 'Xăng',
    seats: 5,
    transmission: 'Tự động 9 cấp',
    origin: 'Đức',
    desc: 'Sedan sang trọng, công nghệ MBUX hiện đại.'
  }
];

// Dữ liệu phụ kiện (có tương thích dòng xe)
const accessoriesData = [
  {
    id: 101,
    name: 'Ghế Da Cao Cấp',
    category: 'Nội thất',
    price: '15.000.000 ₫',
    img: '🪑',
    compat: ['Toyota Camry', 'Honda CR-V', 'Mazda CX-5', 'BMW 320i', 'Mercedes C200'],
    desc: 'Ghế da bò thật, may thủ công, có sưởi và làm mát.',
    material: 'Da bò thật',
    warranty: '24 tháng'
  },
  {
    id: 102,
    name: 'Màn hình Android 12"',
    category: 'Nội thất',
    price: '8.500.000 ₫',
    img: '📺',
    compat: ['Toyota Camry', 'Honda CR-V', 'Ford Ranger', 'Mazda CX-5'],
    desc: 'Màn hình Android 12 inch, chip xử lý mạnh, tích hợp CarPlay/Android Auto.',
    material: 'Kính cường lực',
    warranty: '12 tháng'
  },
  {
    id: 103,
    name: 'Thảm Lót Sàn 5D',
    category: 'Nội thất',
    price: '2.200.000 ₫',
    img: '🧽',
    compat: ['Toyota Camry', 'Honda CR-V', 'Mazda CX-5', 'Ford Ranger', 'Hyundai'],
    desc: 'Thảm da lót sàn 5D chống thấm, vừa khít theo xe.',
    material: 'Da tổng hợp',
    warranty: '6 tháng'
  },
  {
    id: 104,
    name: 'Cánh Gió Thể Thao',
    category: 'Ngoại thất',
    price: '5.500.000 ₫',
    img: '💨',
    compat: ['Honda CR-V', 'Mazda CX-5', 'BMW 320i', 'Ford Ranger'],
    desc: 'Cánh gió carbon fiber, tăng tính khí động học và thẩm mỹ.',
    material: 'Carbon Fiber',
    warranty: '12 tháng'
  },
  {
    id: 105,
    name: 'Camera 360 Độ',
    category: 'An toàn',
    price: '7.800.000 ₫',
    img: '📹',
    compat: ['Toyota Camry', 'Honda CR-V', 'Ford Ranger', 'Mazda CX-5', 'BMW 320i', 'Mercedes C200'],
    desc: 'Camera 360 độ, hiển thị góc nhìn toàn cảnh, hỗ trợ đỗ xe.',
    material: 'Cảm biến CMOS',
    warranty: '18 tháng'
  },
  {
    id: 106,
    name: 'Lốp Michelin Pilot Sport',
    category: 'Vận hành',
    price: '4.200.000 ₫/chiếc',
    img: '⚙️',
    compat: ['Toyota Camry', 'BMW 320i', 'Mercedes C200'],
    desc: 'Lốp thể thao cao cấp, bám đường tốt, giảm ồn.',
    material: 'Cao su tổng hợp',
    warranty: '36 tháng'
  },
  {
    id: 107,
    name: 'Ắc Quy Khô 12V',
    category: 'Vận hành',
    price: '2.900.000 ₫',
    img: '🔋',
    compat: ['Toyota Camry', 'Honda CR-V', 'Ford Ranger', 'Mazda CX-5', 'BMW 320i', 'Mercedes C200'],
    desc: 'Ắc quy khô 12V 70Ah, tuổi thọ cao, khởi động mạnh.',
    material: 'Chì canxi',
    warranty: '24 tháng'
  },
  {
    id: 108,
    name: 'Vô Lăng Bọc Da',
    category: 'Nội thất',
    price: '1.800.000 ₫',
    img: '🎡',
    compat: ['Toyota Camry', 'Honda CR-V', 'Mazda CX-5', 'Ford Ranger', 'Hyundai'],
    desc: 'Vô lăng bọc da may tay, chống trượt, cảm giác cầm chắc.',
    material: 'Da PU cao cấp',
    warranty: '6 tháng'
  }
];