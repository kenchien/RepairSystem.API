// API服務模塊
const API_BASE_URL = '/api';
let token = localStorage.getItem('auth_token');

// 配置axios
axios.defaults.baseURL = API_BASE_URL;
axios.defaults.headers.common['Content-Type'] = 'application/json';

// 請求攔截器 - 添加授權標頭
axios.interceptors.request.use(
  config => {
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

// 響應攔截器 - 處理錯誤
axios.interceptors.response.use(
  response => response,
  error => {
    const { status } = error.response || {};
    
    // 處理401未授權錯誤
    if (status === 401) {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('user_info');
      window.location.href = '/spa/index.html#/login';
    }
    
    return Promise.reject(error);
  }
);

// 身份驗證API
const authApi = {
  login: async (username, password) => {
    const response = await axios.post('/auth/login', { username, password });
    token = response.data.token;
    localStorage.setItem('auth_token', token);
    localStorage.setItem('user_info', JSON.stringify(response.data.user));
    return response.data;
  },
  
  logout: () => {
    token = null;
    localStorage.removeItem('auth_token');
    localStorage.removeItem('user_info');
  },
  
  getCurrentUser: () => {
    const userInfo = localStorage.getItem('user_info');
    return userInfo ? JSON.parse(userInfo) : null;
  }
};

// 報修工單API
const repairApi = {
  getTickets: async (params) => {
    const response = await axios.get('/repairs', { params });
    return response.data;
  },
  
  getTicketById: async (id) => {
    const response = await axios.get(`/repairs/${id}`);
    return response.data;
  },
  
  createTicket: async (ticket) => {
    const response = await axios.post('/repairs', ticket);
    return response.data;
  },
  
  updateTicket: async (id, ticket) => {
    const response = await axios.put(`/repairs/${id}`, ticket);
    return response.data;
  },
  
  deleteTicket: async (id) => {
    const response = await axios.delete(`/repairs/${id}`);
    return response.data;
  },
  
  uploadAttachment: async (ticketId, formData) => {
    const response = await axios.post(`/repairs/${ticketId}/attachments`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  }
};

// 設備API
const equipmentApi = {
  getEquipment: async (params) => {
    const response = await axios.get('/equipment', { params });
    return response.data;
  },
  
  getEquipmentById: async (id) => {
    const response = await axios.get(`/equipment/${id}`);
    return response.data;
  },
  
  createEquipment: async (equipment) => {
    const response = await axios.post('/equipment', equipment);
    return response.data;
  },
  
  updateEquipment: async (id, equipment) => {
    const response = await axios.put(`/equipment/${id}`, equipment);
    return response.data;
  },
  
  deleteEquipment: async (id) => {
    const response = await axios.delete(`/equipment/${id}`);
    return response.data;
  },
  
  getDeviceTypes: async () => {
    const response = await axios.get('/equipment/types');
    return response.data;
  }
};

// 用戶API
const userApi = {
  getUsers: async () => {
    const response = await axios.get('/users');
    return response.data;
  },
  
  getTechnicians: async () => {
    const response = await axios.get('/users/technicians');
    return response.data;
  },
  
  getUserById: async (id) => {
    const response = await axios.get(`/users/${id}`);
    return response.data;
  },
  
  createUser: async (user) => {
    const response = await axios.post('/users', user);
    return response.data;
  },
  
  updateUser: async (id, user) => {
    const response = await axios.put(`/users/${id}`, user);
    return response.data;
  }
};

// 導出所有API模塊
const api = {
  auth: authApi,
  repair: repairApi,
  equipment: equipmentApi,
  user: userApi
}; 