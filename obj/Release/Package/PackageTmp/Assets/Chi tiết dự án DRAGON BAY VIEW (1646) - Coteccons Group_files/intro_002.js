        var iJs = introJs();
        function intro() {
            //Lấy bước đang xem
            let step = getCookie(GMS_INTRO_STEP);
            if (!hasValue(step)) step = 0;
            setCookie(GMS_INTRO, 'intro', 1);

            //Trường hợp đang ở trang khác thì chuyển về trang chủ
            let url = location.pathname;
            if (url != "/" && url.indexOf("Dashboard") == -1) {
                location.href = "/";
            }
            else {
                iJs.setOptions({
                    steps: [
                        {
                            element: "#togleNavbar",
                            intro: "Nhấn để Đóng/Mở menu trái.",
                            position: 'bottom'
                        },
                        {
                            element: '#pageTitle',
                            intro: 'Tiêu đề của trang đang xem.',
                            position: 'bottom'
                        },
                        {
                            element: '#userInfo',
                            intro: 'Thông tin người dùng đăng nhập.',
                            position: 'right'
                        },
                        {
                            element: '#leftIntro',
                            intro: 'Nhấn [v] để Đóng/Mở menu đang xem.',
                            position: 'left'
                        },
                        {
                            element: '#chartIntroIcon',
                            intro: 'Nhấn [^] Đóng/Mở báo cáo đang xem.',
                            position: 'left'
                        },
                        {
                            element: '#chartIntro',
                            intro: 'Báo cáo biểu đồ:<br/>- Nhấn vào cột để xem chi chiết<br/>- Kéo qua lại để xem đầy đủ.',
                            position: 'right'
                        }
                    ]
                });
                iJs.goToStepNumber(parseInt(step))
                    .start()
                    .onexit(function () {
                        //Xoá cockie set đang xem hướng dẫn info
                        setCookie(GMS_INTRO, 'intro', -1);
                        setCookie(GMS_INTRO_STEP, '', 1);
                    })
                    .onchange(function () {
                        //Change cockie step
                        setCookie(GMS_INTRO_STEP, this._currentStep + 1, 1);
                    })
                var dialog = $("#windowAmountByMonth").data("kendoWindow");
                dialog.bind("close", function () {
                    iJs.addSteps([{
                        element: '#btnAction',
                        intro: 'Các nút chức năng.',
                        position: 'left'
                    }]);
                    iJs.addSteps([{
                        element: '#gridProjectGridModel',
                        intro: 'Danh sách dữ liệu',
                        position: 'left'
                    }]);
                    iJs.addSteps([{
                        element: '#gridProjectGridModel .k-grid-filter',
                        intro: 'Nhấn để lọc dữ liệu.',
                        position: 'left'
                    }]);
                    iJs.addSteps([{
                        element: '#gridProjectGridModel .k-filterable',
                        intro: 'Nhấn vào tiêu đề để sắp xếp dữ liệu.',
                        position: 'left'
                    }]);
                    iJs.addSteps([{
                        element: '#gridProjectGridModel .k-grid-pager',
                        intro: 'Thể hiển các thông tin: <br/>- Số trang dữ liệu<br/>- Số dòng dữ liệu trên một trang <br/>- Chức năng làm mới dữ liệu',
                        position: 'top'
                    }]);
                    iJs.addSteps([{
                        element: '.k-tabstrip-items',
                        intro: 'Các tab dữ liệu.',
                        position: 'top'
                    }]);
                    iJs.addSteps([{
                        element: '#grid_PM',
                        intro: 'Nội dung dữ liệu',
                        position: 'top'
                    }]);
                    iJs.start();
                });
            }
        }
        function addIntroStep_ChartAmount() {
            iJs.addSteps([{
                element: '#windowAmountByMonth',
                intro: 'Chi tiết dữ liệu.',
                position: 'left'
            }])
            iJs.addSteps([{
                element: '.k-window-action',
                intro: 'Nhấn [x] để đóng chi tiết.',
                position: 'left'
            }]).start().onbeforechange(function () {
                let cur = this._currentStep;
                if (cur == 5 && this._direction == "backward") {
                    $("#windowAmountByMonth").data("kendoWindow").close();
                }
                else if (cur == 6 && this._direction == "forward") {
                    $("#windowAmountByMonth").data("kendoWindow").open();
                }

            });
        }
        function createProject() {
            debugger;
            setCookie(GMS_INTRO, 'project-new', 1);
            let url = location.pathname;
            if (url == "/" || url.indexOf("Dashboard") > -1) {
                introJs().setOption("doneLabel", "Tiếp tục")
                    .addSteps([{
                        element: '#btnActionCreate',
                        intro: 'Nhấn Tạo mới.',
                        position: 'top'
                    }]).start().onexit(function () {
                        setCookie(GMS_INTRO, 'project-new', 1);
                        location.href = "/Project/Create";
                    });
            }
            else if (url.indexOf('/Project/Update') == -1 && url.indexOf('/Project/Create') == -1 ) {
                location.href = "/";
            }
            else {
                createProjectInfo();
            }
        }
        var iJs1 = introJs();
        function introAddSub(e) {
            let et = e.target.closest("table").parentNode;
            let inputs = $(et).find("input");
            for (var i = 0; i < inputs.length; i++) {
                let aa = document.querySelectorAll('#' + inputs[i].id)[0].closest("td");
                iJs1.addSteps([{
                    element: aa,
                    intro: "Chọn dữ liệu?",
                    position: 'right'
                }]);
            }

            //Nút save
            iJs1.addSteps([{
                element: $(et).find(".k-grid-update")[0],
                intro: 'Nhấn để lưu.',
                position: 'left'
            }]);
            //Nút sub tiếp theo
            if (et.id != "gridProjectPartnerGridModel") {
                iJs1.addSteps([{
                    element: '#gridProjectPartnerGridModel .btn-action-sub',
                    intro: 'Thêm NTP.',
                    position: 'left'
                }]);
            }
            if (et.id == "gridProjectPartnerGridModel") {
                iJs1.addSteps([{
                    element: '#btnSubmit',
                    intro: 'Nhấn lưu thông tin để tạo mới dự án.',
                    position: 'left'
                }]);
            }
            iJs1.start().onbeforechange(function () {
                validateInput(6, this, '#PositionId');
                validateInput(7, this, '#UserId');
                validateInput(10, this, '#ServiceId');
                validateInput(11, this, '#PartnerId');
            }).onexit(function () {
                setCookie(GMS_INTRO, 'project-new', -1);
            });
        }
        function createProjectInfo() {
            iJs1.setOptions({
                steps: [
                    {
                        element: ".k-upload-button",
                        intro: "Chọn hình ảnh.",
                        position: 'top'
                    },
                    {
                        element: '#ProjectCode',
                        intro: 'Nhập mã dự án.',
                        position: 'left'
                    },
                    {
                        element: '#ShortName',
                        intro: 'Nhập tên ngắn cho dự án.',
                        position: 'left'
                    },
                    {
                        element: document.querySelectorAll('#EmailToUsers')[0].parentNode,
                        intro: 'Chọn người nhận email nhắc nhở.',
                        position: 'left'
                    },
                    {
                        element: '#gridProjectUserGridModel .btn-action-sub',
                        intro: 'Thêm nhân sự phụ trách.',
                        position: 'left'
                    }
                ]
            });
            iJs1.start().onbeforechange(function () {
                validateInput(2, this);
                validateInput(3, this);
                validateInput(4, this, '#EmailToUsers');
            }).onexit(function () {
                setCookie(GMS_INTRO, 'project-new', -1);
            });

        }
        function validateInput(step, that, ele) {
            let cur = that._currentStep;
            if (cur == 11) {
                setCookie(GMS_INTRO, 'project-new', -1);
            }
            if (cur == step) {
                let val = "";
                if (hasValue(ele)) {
                    val = $(ele).val();
                } else {
                    val = that._introItems[cur - 1].element.value;
                }
                if (!hasValue(val)) {
                    $(that._introItems[cur - 1].element).focus();
                    that.previousStep();
                }
            }
        }

        $(function () {
            var introId = getCookie(GMS_INTRO);
            if (introId == 'intro') {
                intro();
            }
            else if (introId == 'user') {
                changeUserInfo();
            }
            else if (introId == 'project-new') {
                createProjectInfo();
            }
        })
        function changeUserInfoTop() {
            if (location.pathname.indexOf("/User/Info") == -1) {
                introJs().setOption("doneLabel", "Tiếp tục")
                    .addSteps([{
                        element: '#userInfo',
                        intro: 'Nhấn để vào màn hình thông tin người dùng.',
                        position: 'left'
                    }]).start().onexit(function () {
                        setCookie(GMS_INTRO, 'user', 1);
                        location.href = "/User/Info";
                    });
            }
            else {
                changeUserInfo();
            }
           
        }
        function changeUserInfo() {
            //Lấy bước đang xem
            let step = getCookie(GMS_INTRO_STEP);
            if (!hasValue(step)) step = 0;

            setCookie(GMS_INTRO, 'user', 1);

            introJs().addSteps([{
                element: '#UserCode',
                intro: 'Mã nhân viên được đồng bộ từ hệ thống nhân sự.',
                position: 'right'
                }])
                .addSteps([{
                    element: '#FirstName',
                    intro: 'Nhập họ của Anh/Chị.<br/> Ví dụ: Nguyễn',
                    position: 'right'
                }])
            .addSteps([{
                element: '#LastName',
                intro: 'Nhập Tên ngắn của Anh/Chị.<br/> Ví dụ: Văn An',
                position: 'right'
                }])
                .addSteps([{
                    element: '#Mobile',
                    intro: 'Nhập số điện thoại của Anh/Chị.',
                    position: 'right'
                }])
                .addSteps([{
                    element: document.querySelectorAll('#Birthday')[0].parentNode,
                    intro: 'Nhập ngày sinh của Anh/Chị.',
                    position: 'right'
                }])
                .addSteps([{
                    element: '#ldapInfo',
                    intro: 'Thông tin tài khoản đăng nhập:<br/>- Dùng email công ty: không thay đổi được. <br/>- Tài khoản khác: có thể thay đổi được.',
                    position: 'right'
                }])
                .addSteps([{
                    element: '#sysInfo',
                    intro: 'Thông tin này do người quản trị cập nhật<br/> người dùng không tự thay đổi được.',
                    position: 'right'
                }])
                .addSteps([{
                    element: '#avatarInfo',
                    intro: 'Thay đổi ảnh đại diện của Anh/Chị. <br/>- Chọn ảnh đại diện <br/>- Nhấn tải lên',
                    position: 'top'
                }])
                .addSteps([{
                    element: '#btnUserInfo',
                    intro: 'Nhấn lưu thông tin để cập nhật.',
                    position: 'left'
                }])
                .goToStepNumber(parseInt(step))
                .start()
                .onexit(function () {
                    setCookie(GMS_INTRO, 'user', -1);
                    setCookie(GMS_INTRO_STEP, '', 1);
                }).onchange(function () {
                    //Change cockie step
                    setCookie(GMS_INTRO_STEP, this._currentStep + 1, 1);
                });
        }
