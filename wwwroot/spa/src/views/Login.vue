<template>
  <div class="login-container">
    <el-card class="login-card">
      <h2 class="login-title">硬體報修系統</h2>
      
      <el-form ref="loginForm" :model="form" :rules="rules" label-width="80px">
        <el-form-item label="用戶名" prop="username">
          <el-input v-model="form.username" placeholder="請輸入用戶名"></el-input>
        </el-form-item>
        
        <el-form-item label="密碼" prop="password">
          <el-input v-model="form.password" type="password" placeholder="請輸入密碼" 
                   @keyup.enter="handleLogin"></el-input>
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="handleLogin" :loading="loading" style="width: 100%">
            登入
          </el-button>
        </el-form-item>
      </el-form>
      
      <p class="text-center mt-3" v-if="errorMessage">
        <span style="color: #f56c6c">{{ errorMessage }}</span>
      </p>
    </el-card>
  </div>
</template>

<script>
export default {
  name: 'Login',
  
  setup() {
    const { ref, reactive } = Vue;
    const loginForm = ref(null);
    const loading = ref(false);
    const errorMessage = ref('');
    
    const form = reactive({
      username: '',
      password: ''
    });
    
    const rules = {
      username: [
        { required: true, message: '請輸入用戶名', trigger: 'blur' }
      ],
      password: [
        { required: true, message: '請輸入密碼', trigger: 'blur' },
        { min: 6, message: '密碼長度不能少於6個字符', trigger: 'blur' }
      ]
    };
    
    return {
      loginForm,
      form,
      rules,
      loading,
      errorMessage
    };
  },
  
  methods: {
    handleLogin() {
      this.loginForm.validate(async (valid) => {
        if (!valid) return;
        
        this.loading = true;
        this.errorMessage = '';
        
        try {
          const response = await this.$api.auth.login(this.form.username, this.form.password);
          this.$store.login(response.user);
          this.$message({
            type: 'success',
            message: '登入成功'
          });
          this.$router.push('/');
        } catch (error) {
          console.error('登入失敗:', error);
          this.errorMessage = error.response?.data?.message || '登入失敗，請檢查用戶名和密碼';
        } finally {
          this.loading = false;
        }
      });
    }
  }
};
</script>

<style scoped>
.text-center {
  text-align: center;
}
.mt-3 {
  margin-top: 15px;
}
</style> 