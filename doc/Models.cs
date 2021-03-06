/* ==============================================================================
 * 功能描述：商品分类表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:47
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class CategoryInfo
    {
        /// <summary>
        /// 获取或设置 商品分类Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 商品分类 父级Id
        /// </summary>
        public int ParentId { get; set; }
        
        /// <summary>
        /// 获取或设置 商品分类状态
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 获取或设置 商品分类排序
        /// </summary>
        public int? Order { get; set; }
        
        /// <summary>
        /// 获取或设置 商品分类名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置 商品分类描述
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}



/* ==============================================================================
 * 功能描述：库存表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:53
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class GoodsInfo
    {
        /// <summary>
        /// 获取或设置 货品Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 所属分类Id
        /// </summary>
        public int CategoryId { get; set; }
        
        /// <summary>
        /// 获取或设置 供应商Id
        /// </summary>
        public int SupplierId { get; set; }
        
        /// <summary>
        /// 获取或设置 商品编码
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// 获取或设置 原厂编码
        /// </summary>
        public string OriginalCode { get; set; }
        
        /// <summary>
        /// 获取或设置 来源类型：寄售：1，进货自有：2
        /// </summary>
        public int SourceType { get; set; }
        
        /// <summary>
        /// 获取或设置 商品名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置 货品状态：1、在库；2、预定；3、售出；4、取回；
        /// </summary>
        public int Status { get; set; }
        
        /// <summary>
        /// 获取或设置 货品图片
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// 获取或设置 货品颜色
        /// </summary>
        public string Color { get; set; }
        
        /// <summary>
        /// 获取或设置 货品成色
        /// </summary>
        public string Quality { get; set; }
        
        /// <summary>
        /// 获取或设置 货品配件
        /// </summary>
        public string Parts { get; set; }
        
        /// <summary>
        /// 获取或设置 货品标价
        /// </summary>
        public decimal? MarkPrice { get; set; }
        
        /// <summary>
        /// 获取或设置 进货价格
        /// </summary>
        public decimal? PrimePrice { get; set; }
        
        /// <summary>
        /// 获取或设置 售出价格
        /// </summary>
        public decimal? SalePrice { get; set; }
        
        /// <summary>
        /// 获取或设置 预付款
        /// </summary>
        public decimal? Prepay { get; set; }
        
        /// <summary>
        /// 获取或设置 折扣金额
        /// </summary>
        public decimal Discount { get; set; }
        
        /// <summary>
        /// 获取或设置 货品描述
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// 获取或设置 被预定时间
        /// </summary>
        public DateTime PrepayDate { get; set; }
        
        /// <summary>
        /// 获取或设置 售出时间
        /// </summary>
        public DateTime SaledDate { get; set; }
        
    }
}



/* ==============================================================================
 * 功能描述：操作日志表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:53
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class OperateLogInfo
    {
        /// <summary>
        /// 获取或设置 日志Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 当前用户Id
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 获取或设置 用户所在店铺Id
        /// </summary>
        public int ShopId { get; set; }
        
        /// <summary>
        /// 获取或设置 操作类型：1、新增；2、修改；3、删除
        /// </summary>
        public int? Type { get; set; }
        
        /// <summary>
        /// 获取或设置 操作描述
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 操作用户
        /// </summary>
        public string Operator { get; set; }
        
        /// <summary>
        /// 获取或设置 操作时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}

/* ==============================================================================
 * 功能描述：付款类型表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:55
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class PayTypeInfo
    {
        /// <summary>
        /// 获取或设置 付款类型Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 付款方式名称
        /// </summary>
        public string PayName { get; set; }
        
    }
}

/* ==============================================================================
 * 功能描述：进货记录表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:55
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class PurchaseRecordInfo
    {
        /// <summary>
        /// 获取或设置 进货Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 当前用户Id
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 获取或设置 货品Id
        /// </summary>
        public int GoodsId { get; set; }
        
        /// <summary>
        /// 获取或设置 付款方式：1、现金；2、汇款；3、信用卡；4、网银转帐；
        /// </summary>
        public int PayType { get; set; }
        
        /// <summary>
        /// 获取或设置 描述信息
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 经手人
        /// </summary>
        public string Operator { get; set; }
        
        /// <summary>
        /// 获取或设置 进货时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}

/* ==============================================================================
 * 功能描述：系统角色表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:55
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class RoleInfo
    {
        /// <summary>
        /// 获取或设置 角色Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 角色名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置 角色描述
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}

/* ==============================================================================
 * 功能描述：销售记录
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:56
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class SaledRecordInfo
    {
        /// <summary>
        /// 获取或设置 销售记录Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 当前登录用户名
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 获取或设置 货品Id
        /// </summary>
        public int GoodsId { get; set; }
        
        /// <summary>
        /// 获取或设置 付款方式：1、现金；2、汇款；3、信用卡；4、网银转帐；
        /// </summary>
        public int? PayType { get; set; }
        
        /// <summary>
        /// 获取或设置 销售备注信息
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 经手人
        /// </summary>
        public string Operator { get; set; }
        
        /// <summary>
        /// 获取或设置 售出时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}
/* ==============================================================================
 * 功能描述：店铺表
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:56
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class ShopInfo
    {
        /// <summary>
        /// 获取或设置 店铺Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 店铺名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置 联系电话
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// 获取或设置 店铺地址
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 获取或设置 店铺描述
        /// </summary>
        public string Desc { get; set; }
        
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}
/* ==============================================================================
 * 功能描述：供应商
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:56
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class SupplierInfo
    {
        /// <summary>
        /// 获取或设置 供应商Id
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 获取或设置 供应商名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 获取或设置 供应商性别：1、男;2、女;
        /// </summary>
        public int Sex { get; set; }
        
        /// <summary>
        /// 获取或设置 联系电话
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// 获取或设置 开户行名称
        /// </summary>
        public string BankName { get; set; }
        
        /// <summary>
        /// 获取或设置 银行卡号
        /// </summary>
        public string BankCard { get; set; }
        
        /// <summary>
        /// 获取或设置 身份证号
        /// </summary>
        public string IdCard { get; set; }
        
        /// <summary>
        /// 获取或设置 通信地址
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}
/* ==============================================================================
 * 功能描述：系统用户
 * 创 建 者：zjk
 * 创建日期：2013-10-27 20:57
 * 修改日期：
 * 修改详情：
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace V2IMS.Model
{
    public class UserInfo
    {
        /// <summary>
        /// 获取或设置 用户Id
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 获取或设置 角色Id
        /// </summary>
        public int RoleId { get; set; }
        
        /// <summary>
        /// 获取或设置 所在商铺Id
        /// </summary>
        public int ShopId { get; set; }
        
        /// <summary>
        /// 获取或设置 用户登录名
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// 获取或设置 用户真实姓名
        /// </summary>
        public string RealName { get; set; }
        
        /// <summary>
        /// 获取或设置 登录密码
        /// </summary>
        public string Pwd { get; set; }
        
        /// <summary>
        /// 获取或设置 性别：1、男;2、女;
        /// </summary>
        public int? Sex { get; set; }
        
        /// <summary>
        /// 获取或设置 联系电话
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// 获取或设置 创建时间
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
    }
}

