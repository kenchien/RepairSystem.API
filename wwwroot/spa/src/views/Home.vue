<template>
  <div class="home-container">
    <el-row :gutter="20">
      <el-col :span="24">
        <h2 class="page-title">歡迎使用硬體報修系統</h2>
        <p v-if="store.user">{{ getGreeting() }}，{{ store.user.name }}！</p>
      </el-col>
    </el-row>
    
    <el-row :gutter="20" class="mt-4">
      <!-- 數據統計卡片 -->
      <el-col :xs="24" :sm="12" :md="8" :lg="6" v-for="stat in stats" :key="stat.title">
        <el-card class="stat-card" :body-style="{ padding: '20px' }">
          <div class="stat-icon" :class="stat.iconClass">
            <i :class="stat.icon"></i>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ stat.value }}</div>
            <div class="stat-title">{{ stat.title }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 快捷功能區 -->
    <el-row :gutter="20" class="mt-4">
      <el-col :span="24">
        <h3>快捷功能</h3>
      </el-col>
    </el-row>
    
    <el-row :gutter="20" class="mt-2">
      <el-col :xs="24" :sm="12" :md="8" v-for="action in quickActions" :key="action.title">
        <el-card class="action-card" :body-style="{ padding: '20px' }" @click="navigateTo(action.route)">
          <div class="action-content">
            <div class="action-icon">
              <i :class="action.icon"></i>
            </div>
            <div class="action-title">{{ action.title }}</div>
            <div class="action-description">{{ action.description }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <!-- 最近工單區 -->
    <el-row :gutter="20" class="mt-4" v-if="recentTickets.length > 0">
      <el-col :span="24">
        <h3>最近工單</h3>
        <el-table :data="recentTickets" border style="width: 100%" class="mt-2">
          <el-table-column prop="id" label="工單號" width="80"></el-table-column>
          <el-table-column prop="title" label="標題"></el-table-column>
          <el-table-column prop="status" label="狀態" width="120">
            <template #default="scope">
              <el-tag :type="getStatusType(scope.row.status)">{{ scope.row.status }}</el-tag>
            </template>
          </el-table-column>
          <el-table-column prop="createdAt" label="創建時間" width="170">
            <template #default="scope">
              {{ formatDate(scope.row.createdAt) }}
            </template>
          </el-table-column>
          <el-table-column label="操作" width="120">
            <template #default="scope">
              <el-button size="small" @click="navigateTo(`/repairs/${scope.row.id}`)">
                查看
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-col>
    </el-row>
  </div>
</template>

<script>
export default {
  name: 'Home',
  
  setup() {
    const { ref, reactive, computed, onMounted } = Vue;
    const store = inject('store');
    const router = inject('router');
    const api = inject('api');
    
    const stats = ref([]);
    const recentTickets = ref([]);
    const loading = ref(false);
    
    const quickActions = computed(() => {
      const actions = [
        {
          title: '提交維修申請',
          description: '報告設備故障並申請維修服務',
          icon: 'el-icon-plus',
          route: '/repairs/new'
        },
        {
          title: '查看我的工單',
          description: '查看您提交的所有維修工單',
          icon: 'el-icon-document',
          route: '/repairs'
        }
      ];
      
      if (store.state.isAdmin || store.state.isTechnician) {
        actions.push({
          title: '處理維修工單',
          description: '查看並處理待處理的維修工單',
          icon: 'el-icon-s-cooperation',
          route: '/repairs'
        });
      }
      
      if (store.state.isAdmin) {
        actions.push({
          title: '設備管理',
          description: '管理設備及設備類型',
          icon: 'el-icon-s-platform',
          route: '/equipment'
        });
        
        actions.push({
          title: '用戶管理',
          description: '管理系統用戶和權限',
          icon: 'el-icon-user',
          route: '/users'
        });
      }
      
      return actions;
    });
    
    onMounted(async () => {
      loading.value = true;
      try {
        await loadStats();
        await loadRecentTickets();
      } catch (error) {
        console.error('加載首頁數據失敗:', error);
      } finally {
        loading.value = false;
      }
    });
    
    const loadStats = async () => {
      // 這裡應該向API請求統計數據，現在使用模擬數據
      stats.value = [
        { title: '總工單數', value: '208', icon: 'el-icon-tickets', iconClass: 'blue' },
        { title: '待處理', value: '15', icon: 'el-icon-time', iconClass: 'orange' },
        { title: '處理中', value: '32', icon: 'el-icon-loading', iconClass: 'cyan' },
        { title: '已完成', value: '161', icon: 'el-icon-check', iconClass: 'green' }
      ];
    };
    
    const loadRecentTickets = async () => {
      try {
        // 這裡應該使用真實API數據
        recentTickets.value = await api.repair.getTickets({ limit: 5 });
      } catch (error) {
        console.error('加載最近工單失敗:', error);
        // 使用模擬數據
        recentTickets.value = [
          { id: 101, title: '打印機故障', status: '待處理', createdAt: '2023-03-08T08:30:00' },
          { id: 100, title: '無法連接網絡', status: '處理中', createdAt: '2023-03-07T14:25:00' },
          { id: 99, title: '顯示器無信號', status: '已完成', createdAt: '2023-03-06T09:15:00' }
        ];
      }
    };
    
    const getStatusType = (status) => {
      const statusMap = {
        '待處理': 'warning',
        '處理中': 'primary',
        '已完成': 'success',
        '已取消': 'info'
      };
      return statusMap[status] || 'info';
    };
    
    const formatDate = (dateString) => {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleString('zh-TW');
    };
    
    const getGreeting = () => {
      const hour = new Date().getHours();
      if (hour < 6) return '凌晨好';
      if (hour < 12) return '早上好';
      if (hour < 14) return '中午好';
      if (hour < 18) return '下午好';
      return '晚上好';
    };
    
    const navigateTo = (route) => {
      router.push(route);
    };
    
    return {
      store: store.state,
      stats,
      quickActions,
      recentTickets,
      loading,
      getStatusType,
      formatDate,
      getGreeting,
      navigateTo
    };
  }
};
</script>

<style scoped>
.home-container {
  padding: 20px;
}
.mt-2 {
  margin-top: 10px;
}
.mt-4 {
  margin-top: 20px;
}
.stat-card {
  height: 100px;
  display: flex;
  align-items: center;
  cursor: default;
  transition: all 0.3s;
}
.stat-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0,0,0,0.1);
}
.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 15px;
  font-size: 24px;
  color: #fff;
}
.stat-icon.blue { background-color: #409EFF; }
.stat-icon.green { background-color: #67C23A; }
.stat-icon.orange { background-color: #E6A23C; }
.stat-icon.red { background-color: #F56C6C; }
.stat-icon.cyan { background-color: #17a2b8; }
.stat-value {
  font-size: 24px;
  font-weight: bold;
}
.stat-title {
  font-size: 14px;
  color: #909399;
}
.action-card {
  height: 150px;
  margin-bottom: 20px;
  cursor: pointer;
  transition: all 0.3s;
}
.action-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0,0,0,0.1);
}
.action-content {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}
.action-icon {
  font-size: 24px;
  color: #409EFF;
  margin-bottom: 10px;
}
.action-title {
  font-size: 18px;
  font-weight: bold;
  margin-bottom: 5px;
}
.action-description {
  font-size: 14px;
  color: #909399;
}
</style> 