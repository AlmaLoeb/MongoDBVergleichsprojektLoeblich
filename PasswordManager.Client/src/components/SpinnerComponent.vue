<template>
    <div class="spinner">

    </div>
</template>
<style scoped>
.spinner {
    border: 16px solid silver;
    border-top: 16px solid #9d57da;
    border-radius: 50%;
    width: 80px;
    height: 80px;
    animation: spin 700ms linear infinite;
    top: 50%;
    left: 50%;
    margin-top: -40px;
    margin-left: -40px;
    position: absolute;
}

@keyframes spin {
    0% {
        transform: rotate(0deg)
    }

    100% {
        transform: rotate(360deg)
    }
}
</style>