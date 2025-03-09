// 導入Vue相關功能
const { createApp, ref, reactive, computed, onMounted } = Vue;
const { createRouter, createWebHashHistory } = VueRouter;

// 導入API服務
const { auth, repair, equipment, user } = api;

// 導入Element Plus組件
const { 
  ElButton, ElInput, ElForm, ElFormItem, ElTable, ElTableColumn, 
  ElSelect, ElOption, ElDatePicker, ElDialog, ElTag, ElMessage,
  ElMenu, ElMenuItem, ElSubmenu, ElBreadcrumb, ElBreadcrumbItem,
  ElCard, ElPopconfirm, ElUpload, ElLoading, ElPagination
} = ElementPlus;

// 全局狀態管理
const store = {
  state: reactive({
    user: null,
    isLoggedIn: false,
    isAdmin: false,
    isTechnician: false
  }),
  
  init() {
    const currentUser = auth.getCurrentUser();
    if (currentUser) {
      this.state.user = currentUser;
      this.state.isLoggedIn = true;
      this.state.isAdmin = currentUser.role === 'Admin';
      this.state.isTechnician = currentUser.role === 'Technician';
    }
  },
  
  login(userData) {
    this.state.user = userData;
    this.state.isLoggedIn = true;
    this.state.isAdmin = userData.role === 'Admin';
    this.state.isTechnician = userData.role === 'Technician';
  },
  
  logout() {
    auth.logout();
    this.state.user = null;
    this.state.isLoggedIn = false;
    this.state.isAdmin = false;
    this.state.isTechnician = false;
    router.push('/login');
  }
};

// 初始化全局狀態
store.init();

// 路由配置
const routes = [
  { 
    path: '/', 
    component: httpVueLoader('./src/views/Home.vue'),
    meta: { requiresAuth: true }
  },
  { 
    path: '/login', 
    component: httpVueLoader('./src/views/Login.vue'),
    meta: { guest: true }
  },
  { 
    path: '/repairs', 
    component: httpVueLoader('./src/views/RepairList.vue'),
    meta: { requiresAuth: true }
  },
  { 
    path: '/repairs/new', 
    component: httpVueLoader('./src/views/RepairForm.vue'),
    meta: { requiresAuth: true }
  },
  { 
    path: '/repairs/:id', 
    component: httpVueLoader('./src/views/RepairDetail.vue'),
    meta: { requiresAuth: true },
    props: true
  },
  { 
    path: '/repairs/:id/edit', 
    component: httpVueLoader('./src/views/RepairForm.vue'),
    meta: { requiresAuth: true },
    props: true
  },
  { 
    path: '/equipment', 
    component: httpVueLoader('./src/views/EquipmentList.vue'),
    meta: { requiresAuth: true }
  },
  { 
    path: '/equipment/new', 
    component: httpVueLoader('./src/views/EquipmentForm.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  { 
    path: '/equipment/:id/edit', 
    component: httpVueLoader('./src/views/EquipmentForm.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
    props: true
  },
  { 
    path: '/users', 
    component: httpVueLoader('./src/views/UserList.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  { 
    path: '/users/new', 
    component: httpVueLoader('./src/views/UserForm.vue'),
    meta: { requiresAuth: true, requiresAdmin: true }
  },
  { 
    path: '/users/:id/edit', 
    component: httpVueLoader('./src/views/UserForm.vue'),
    meta: { requiresAuth: true, requiresAdmin: true },
    props: true
  },
  { 
    path: '/profile', 
    component: httpVueLoader('./src/views/UserProfile.vue'),
    meta: { requiresAuth: true }
  },
  { 
    path: '/:pathMatch(.*)*', 
    component: httpVueLoader('./src/views/NotFound.vue')
  }
];

const router = createRouter({
  history: createWebHashHistory(),
  routes
});

// 路由守衛
router.beforeEach((to, from, next) => {
  const isLoggedIn = store.state.isLoggedIn;
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth);
  const requiresAdmin = to.matched.some(record => record.meta.requiresAdmin);
  const isAdmin = store.state.isAdmin;
  const isForGuests = to.matched.some(record => record.meta.guest);
  
  if (requiresAuth && !isLoggedIn) {
    next('/login');
  } else if (isForGuests && isLoggedIn) {
    next('/');
  } else if (requiresAdmin && !isAdmin) {
    next('/');
  } else {
    next();
  }
});

// 創建Vue應用
const app = createApp({
  setup() {
    return {
      store: store.state
    };
  },
  methods: {
    logout() {
      store.logout();
    }
  },
  template: `
    <div class="app-container">
      <el-menu mode="horizontal" v-if="store.isLoggedIn" router>
        <el-menu-item index="/">首頁</el-menu-item>
        <el-menu-item index="/repairs">維修申請</el-menu-item>
        <el-menu-item index="/equipment">設備管理</el-menu-item>
        <el-menu-item index="/users" v-if="store.isAdmin">用戶管理</el-menu-item>
        <el-menu-item index="/profile" style="margin-left: auto;">
          {{ store.user ? store.user.name : '個人資料' }}
        </el-menu-item>
        <el-menu-item @click="logout">登出</el-menu-item>
      </el-menu>
      
      <div class="main-content">
        <router-view></router-view>
      </div>
    </div>
  `
});

// 注冊組件
app.component('el-button', ElButton);
app.component('el-input', ElInput);
app.component('el-form', ElForm);
app.component('el-form-item', ElFormItem);
app.component('el-table', ElTable);
app.component('el-table-column', ElTableColumn);
app.component('el-select', ElSelect);
app.component('el-option', ElOption);
app.component('el-date-picker', ElDatePicker);
app.component('el-dialog', ElDialog);
app.component('el-tag', ElTag);
app.component('el-menu', ElMenu);
app.component('el-menu-item', ElMenuItem);
app.component('el-submenu', ElSubmenu);
app.component('el-breadcrumb', ElBreadcrumb);
app.component('el-breadcrumb-item', ElBreadcrumbItem);
app.component('el-card', ElCard);
app.component('el-popconfirm', ElPopconfirm);
app.component('el-upload', ElUpload);
app.component('el-pagination', ElPagination);

// 註冊全局屬性
app.config.globalProperties.$message = ElMessage;
app.config.globalProperties.$loading = ElLoading.service;
app.config.globalProperties.$api = api;
app.config.globalProperties.$store = store;

// 掛載路由
app.use(router);

// 掛載應用
app.mount('#app'); 