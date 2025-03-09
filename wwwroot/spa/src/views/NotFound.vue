<template>
  <div class="not-found-container">
    <div class="not-found-content">
      <h1 class="not-found-title">404</h1>
      <p class="not-found-text">很抱歉，您所訪問的頁面不存在</p>
      <el-button type="primary" @click="$router.push('/')">返回首頁</el-button>
    </div>
  </div>
</template>

<script>
export default {
  name: 'NotFound'
};
</script>

<style scoped>
.not-found-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 80vh;
  text-align: center;
}
.not-found-content {
  max-width: 500px;
  padding: 20px;
}
.not-found-title {
  font-size: 120px;
  margin: 0;
  color: #409EFF;
}
.not-found-text {
  font-size: 20px;
  margin: 20px 0 30px;
  color: #606266;
}
</style> 