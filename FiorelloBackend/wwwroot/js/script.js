$(document).ready(function () {

 
    //add product to basket

    let addBasketBtns = document.querySelectorAll(".add-basket");

    addBasketBtns.forEach((btn) => {
        btn.addEventListener("click", function () {
            let productId = parseInt(this.getAttribute("data-id"));
            fetch("https://localhost:7040/Home/AddProductToBasket?id=" + productId, {
                method: "POST",
                headers: {
                    "Content-type": "application/json; charset=UTF-8"
                }
            }).then(response => response.text()).then(res => {
                document.querySelector(".basket-count-show").innerText = res;
                Swal.fire({
                    position: "top-right",
                    icon: "success",
                    title: "Product added to basket",
                    showConfirmButton: false,
                    timer: 1500
                });
            });  
        })
    })

    //delete product from basket

    let deleteBasketBtns = document.querySelectorAll(".delete-basket-item");

    deleteBasketBtns.forEach((btn) => {
        btn.addEventListener("click", function () {
            Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    let productId = parseInt(this.getAttribute("data-id"));
                    fetch("https://localhost:7040/Cart/Delete?id=" + productId, {
                        method: "POST",
                        headers: {
                            "Content-type": "application/json; charset=UTF-8"
                        }
                    }).then(response => response.json()).then(res => {
                        this.parentNode.parentNode.remove();
                        document.querySelector(".total h2 span").innerText = res.total;
                        document.querySelector(".basket-count-show").innerText = res.count;
                        if (res.count == 0) {
                            document.querySelector(".cart-area").classList.add("d-none");
                            document.querySelector(".cart-empty-alert").classList.remove("d-none");
                        }
                    });
                }
            });
         
        })
    })

    // HEADER

    $(document).on('click', '#search', function () {
        $(this).next().toggle();
    })

    $(document).on('click', '#mobile-navbar-close', function () {
        $(this).parent().removeClass("active");

    })
    $(document).on('click', '#mobile-navbar-show', function () {
        $('.mobile-navbar').addClass("active");

    })

    $(document).on('click', '.mobile-navbar ul li a', function () {
        if ($(this).children('i').hasClass('fa-caret-right')) {
            $(this).children('i').removeClass('fa-caret-right').addClass('fa-sort-down')
        }
        else {
            $(this).children('i').removeClass('fa-sort-down').addClass('fa-caret-right')
        }
        $(this).parent().next().slideToggle();
    })

    // SLIDER

    $(document).ready(function(){
        $(".slider").owlCarousel(
            {
                items: 1,
                loop: true,
                autoplay: true
            }
        );
      });

    // PRODUCT

    $(document).on('click', '.categories', function(e)
    {
        e.preventDefault();
        $(this).next().next().slideToggle();
    })

    $(document).on('click', '.category li a', function (e) {
        e.preventDefault();
        let category = $(this).attr('data-id');
        let products = $('.product-item');
        
        products.each(function () {
            if(category == $(this).attr('data-id'))
            {
                $(this).parent().fadeIn();
            }
            else
            {
                $(this).parent().hide();
            }
        })
        if(category == 'all')
        {
            products.parent().fadeIn();
        }
    })

    // ACCORDION 

    $(document).on('click', '.question', function()
    {   
       $(this).siblings('.question').children('i').removeClass('fa-minus').addClass('fa-plus');
       $(this).siblings('.answer').not($(this).next()).slideUp();
       $(this).children('i').toggleClass('fa-plus').toggleClass('fa-minus');
       $(this).next().slideToggle();
       $(this).siblings('.active').removeClass('active');
       $(this).toggleClass('active');
    })

    // TAB

    $(document).on('click', 'ul li', function()
    {   
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().next().children('p.active').removeClass('active');

        $(this).parent().next().children('p').each(function()
        {
            if(dataId == $(this).attr('data-id'))
            {
                $(this).addClass('active')
            }
        })
    })

    $(document).on('click', '.tab4 ul li', function()
    {   
        $(this).siblings('.active').removeClass('active');
        $(this).addClass('active');
        let dataId = $(this).attr('data-id');
        $(this).parent().parent().next().children().children('p.active').removeClass('active');

        $(this).parent().parent().next().children().children('p').each(function()
        {
            if(dataId == $(this).attr('data-id'))
            {
                $(this).addClass('active')
            }
        })
    })

    // INSTAGRAM

    $(document).ready(function(){
        $(".instagram").owlCarousel(
            {
                items: 4,
                loop: true,
                autoplay: true,
                responsive:{
                    0:{
                        items:1
                    },
                    576:{
                        items:2
                    },
                    768:{
                        items:3
                    },
                    992:{
                        items:4
                    }
                }
            }
        );
      });

      $(document).ready(function(){
        $(".say").owlCarousel(
            {
                items: 1,
                loop: true,
                autoplay: true
            }
        );
      });
})