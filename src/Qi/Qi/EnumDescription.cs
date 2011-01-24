using System;

namespace Qi
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class EnumDescriptionAttribute : Attribute
    {
        public EnumDescriptionAttribute(string description)
        {
            Description = description;
            IsDefault = false;
        }

        public string Description { get; set; }

        /// <summary>
        /// ���û��ȡLanguage������
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// �趨���ȡ�Ƿ�ΪĬ����ʾ��ʽ
        /// </summary>
        public bool IsDefault { get; set; }
    }
}