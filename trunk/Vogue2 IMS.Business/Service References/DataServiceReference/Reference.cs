﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18052
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Vogue2_IMS.Business.DataServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DataServiceReference.IDataService")]
    public interface IDataService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/UserValidator", ReplyAction="http://tempuri.org/IDataService/UserValidatorResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(int[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.DataService.Model.ResultValue[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.Service.Business.Model.FunctionParms[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.Service.Business.Model.FunctionParms))]
        Vogue2_IMS.DataService.Model.ResultValue UserValidator(string userName, string pwd);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/FuncSaveData", ReplyAction="http://tempuri.org/IDataService/FuncSaveDataResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(int[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.DataService.Model.ResultValue[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.Service.Business.Model.FunctionParms[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.Service.Business.Model.FunctionParms))]
        Vogue2_IMS.DataService.Model.ResultValue FuncSaveData(Vogue2_IMS.Service.Business.Model.FunctionParms functionParms);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/FuncBatchSaveData", ReplyAction="http://tempuri.org/IDataService/FuncBatchSaveDataResponse")]
        Vogue2_IMS.DataService.Model.ResultValue[] FuncBatchSaveData(Vogue2_IMS.Service.Business.Model.FunctionParms[] functionParms);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IDataService/FuncGetResults", ReplyAction="http://tempuri.org/IDataService/FuncGetResultsResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(int[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[][]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(object[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, object>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.DataService.Model.ResultValue[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.Service.Business.Model.FunctionParms[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Vogue2_IMS.Service.Business.Model.FunctionParms))]
        Vogue2_IMS.DataService.Model.ResultValue FuncGetResults(Vogue2_IMS.Service.Business.Model.FunctionParms functionParms);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IDataServiceChannel : Vogue2_IMS.Business.DataServiceReference.IDataService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DataServiceClient : System.ServiceModel.ClientBase<Vogue2_IMS.Business.DataServiceReference.IDataService>, Vogue2_IMS.Business.DataServiceReference.IDataService {
        
        public DataServiceClient() {
        }
        
        public DataServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public DataServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public DataServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Vogue2_IMS.DataService.Model.ResultValue UserValidator(string userName, string pwd) {
            return base.Channel.UserValidator(userName, pwd);
        }
        
        public Vogue2_IMS.DataService.Model.ResultValue FuncSaveData(Vogue2_IMS.Service.Business.Model.FunctionParms functionParms) {
            return base.Channel.FuncSaveData(functionParms);
        }
        
        public Vogue2_IMS.DataService.Model.ResultValue[] FuncBatchSaveData(Vogue2_IMS.Service.Business.Model.FunctionParms[] functionParms) {
            return base.Channel.FuncBatchSaveData(functionParms);
        }
        
        public Vogue2_IMS.DataService.Model.ResultValue FuncGetResults(Vogue2_IMS.Service.Business.Model.FunctionParms functionParms) {
            return base.Channel.FuncGetResults(functionParms);
        }
    }
}