"use strict";




//product images deleted
let deleteBtns = document.querySelectorAll(".delete-btn");


deleteBtns.forEach((btn) => {
    btn.addEventListener("click", function () {
        let imageId = parseInt(this.parentNode.getAttribute("data-id"));
        fetch("https://localhost:7040/Admin/Product/DeleteProductImage?id=" + imageId, {
            method: "POST",
            headers: {
                "Content-type": "application/json; charset=UTF-8"
            }
        }).then(response => response.text()).then(res => {
            this.parentNode.parentNode.parentNode.remove();
        });  
    });
});

//product deleted
let deleteProductBtns = document.querySelectorAll(".delete-product");


deleteProductBtns.forEach((btn) => {
    btn.addEventListener("click", function () {
        let productId = parseInt(this.getAttribute("data-id"));
        fetch("https://localhost:7040/Admin/Product/Delete?id=" + productId, {
            method: "POST",
            headers: {
                "Content-type": "application/json; charset=UTF-8"
            }
        }).then(response => response.text()).then(res => {
            this.parentNode.parentNode.remove()
        });
    });
});