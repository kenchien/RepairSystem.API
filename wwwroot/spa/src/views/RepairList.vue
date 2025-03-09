<template>
  <div class="repair-list-container page-container">
    <div class="page-header">
      <h2 class="page-title">維修工單管理</h2>
      <el-button type="primary" @click="$router.push('/repairs/new')" class="add-button">
        新增工單
      </el-button>
    </div>
    
    <!-- 搜尋和篩選 -->
    <div class="search-bar">
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="工單號">
          <el-input v-model="searchForm.id" placeholder="輸入工單號" clearable></el-input>
        </el-form-item>
        <el-form-item label="標題">
          <el-input v-model="searchForm.title" placeholder="輸入標題關鍵字" clearable></el-input>
        </el-form-item>
        <el-form-item label="狀態">
          <el-select v-model="searchForm.status" placeholder="請選擇" clearable>
            <el-option label="待處理" value="待處理"></el-option>
            <el-option label="處理中" value="處理中"></el-option>
            <el-option label="已完成" value="已完成"></el-option>
            <el-option label="已取消" value="已取消"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜尋</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </div>
    
    <!-- 工單表格 -->
    <el-table 
      v-loading="loading" 
      :data="tickets" 
      border 
      style="width: 100%" 
      @row-click="handleRowClick">
      <el-table-column prop="id" label="工單號" width="80"></el-table-column>
      <el-table-column prop="title" label="標題"></el-table-column>
      <el-table-column prop="equipmentName" label="設備名稱" width="150"></el-table-column>
      <el-table-column prop="priority" label="優先級" width="100">
        <template #default="scope">
          <el-tag 
            :type="getPriorityType(scope.row.priority)" 
            size="small">
            {{ scope.row.priority }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="狀態" width="100">
        <template #default="scope">
          <el-tag 
            :type="getStatusType(scope.row.status)" 
            size="small">
            {{ scope.row.status }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createdAt" label="創建時間" width="170">
        <template #default="scope">
          {{ formatDate(scope.row.createdAt) }}
        </template>
      </el-table-column>
      <el-table-column prop="createdByName" label="創建人" width="120"></el-table-column>
      <el-table-column prop="handledByName" label="處理人" width="120"></el-table-column>
      <el-table-column label="操作" width="150" fixed="right">
        <template #default="scope">
          <el-button 
            size="small" 
            @click.stop="$router.push(`/repairs/${scope.row.id}`)">
            查看
          </el-button>
          <el-button 
            size="small" 
            type="primary" 
            @click.stop="$router.push(`/repairs/${scope.row.id}/edit`)"
            v-if="canEdit(scope.row)">
            編輯
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    
    <!-- 分頁 -->
    <div class="pagination-container">
      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="pagination.page"
        :page-sizes="[10, 20, 50, 100]"
        :page-size="pagination.pageSize"
        layout="total, sizes, prev, pager, next, jumper"
        :total="pagination.total">
      </el-pagination>
    </div>
  </div>
</template>

<script>
export default {
  name: 'RepairList',
  
  setup() {
    const { ref, reactive, onMounted, inject } = Vue;
    const api = inject('api');
    const store = inject('store');
    const message = inject('message');
    
    const tickets = ref([]);
    const loading = ref(false);
    
    const searchForm = reactive({
      id: '',
      title: '',
      status: '',
      priority: ''
    });
    
    const pagination = reactive({
      page: 1,
      pageSize: 10,
      total: 0
    });
    
    onMounted(() => {
      loadTickets();
    });
    
    const loadTickets = async () => {
      loading.value = true;
      try {
        const params = {
          page: pagination.page,
          pageSize: pagination.pageSize,
          ...searchForm
        };
        
        // 如果不是管理員或技術員，只查詢自己的工單
        if (!store.state.isAdmin && !store.state.isTechnician) {
          params.userId = store.state.user.id;
        }
        
        const response = await api.repair.getTickets(params);
        tickets.value = response.items || [];
        pagination.total = response.total || 0;
      } catch (error) {
        console.error('加載工單失敗:', error);
        message.error('加載工單失敗');
        
        // 使用模擬數據
        tickets.value = [
          { 
            id: 101, 
            title: '打印機故障', 
            equipmentName: '辦公室打印機',
            priority: '高',
            status: '待處理', 
            createdAt: '2023-03-08T08:30:00',
            createdByName: '張三',
            handledByName: ''
          },
          { 
            id: 100, 
            title: '無法連接網絡', 
            equipmentName: '筆記本電腦', 
            priority: '中',
            status: '處理中', 
            createdAt: '2023-03-07T14:25:00',
            createdByName: '李四',
            handledByName: '王技術員'
          },
          { 
            id: 99, 
            title: '顯示器無信號', 
            equipmentName: '辦公室顯示器',
            priority: '低',
            status: '已完成', 
            createdAt: '2023-03-06T09:15:00',
            createdByName: '王五',
            handledByName: '張技術員'
          }
        ];
        pagination.total = tickets.value.length;
      } finally {
        loading.value = false;
      }
    };
    
    const handleSearch = () => {
      pagination.page = 1;
      loadTickets();
    };
    
    const resetSearch = () => {
      Object.keys(searchForm).forEach(key => {
        searchForm[key] = '';
      });
      pagination.page = 1;
      loadTickets();
    };
    
    const handleSizeChange = (val) => {
      pagination.pageSize = val;
      loadTickets();
    };
    
    const handleCurrentChange = (val) => {
      pagination.page = val;
      loadTickets();
    };
    
    const handleRowClick = (row) => {
      router.push(`/repairs/${row.id}`);
    };
    
    const canEdit = (ticket) => {
      if (store.state.isAdmin) return true;
      if (store.state.isTechnician && 
          (ticket.status === '待處理' || ticket.status === '處理中')) return true;
      if (store.state.user && store.state.user.id === ticket.createdBy && 
          ticket.status === '待處理') return true;
      return false;
    };
    
    const getPriorityType = (priority) => {
      const priorityMap = {
        '高': 'danger',
        '中': 'warning',
        '低': 'info'
      };
      return priorityMap[priority] || 'info';
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
    
    return {
      tickets,
      loading,
      searchForm,
      pagination,
      handleSearch,
      resetSearch,
      handleSizeChange,
      handleCurrentChange,
      handleRowClick,
      canEdit,
      getPriorityType,
      getStatusType,
      formatDate
    };
  }
};
</script>

<style scoped>
.repair-list-container {
  padding: 20px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.search-bar {
  margin-bottom: 20px;
  padding-bottom: 20px;
  border-bottom: 1px solid var(--border-color);
}
.search-form {
  display: flex;
  flex-wrap: wrap;
}
.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style> 