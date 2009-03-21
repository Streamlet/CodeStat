using System;
using System.Collections.Generic;
using System.Text;

namespace Streamlet.CodeStat
{
    /// <summary>
    /// 统计数据模型
    /// </summary>
    public class StatModel
    {
        public StatModel(string filename)
        {
            _filename = filename;
            _codeLines = 0;
            _commentLines = 0;
            _blankLines = 0;
            _isValid = false;
        }

        private bool _isValid;
        /// <summary>
        /// 数据是否有效，初始为 false。当后面的 BlankLines、CodeLines 或者 CommentLines 被改变过时变为 true。
        /// </summary>
        public bool IsValid
        {
            get { return _isValid; }
        }

        private string _filename;
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName
        {
            get { return _filename; }
        }

        /// <summary>
        /// 总代码量
        /// </summary>
        public int TotalLines
        {
            get { return _codeLines + _commentLines + _blankLines; }
        }

        private int _codeLines;
        /// <summary>
        /// 有效代码行数
        /// </summary>
        public int CodeLines
        {
            get { return _codeLines; }
            set { _isValid = true; _codeLines = value; }
        }

        private int _commentLines;
        /// <summary>
        /// 注释行数
        /// </summary>
        public int CommentLines
        {
            get { return _commentLines; }
            set { _isValid = true; _commentLines = value; }
        }

        private int _blankLines;
        /// <summary>
        /// 空白行数
        /// </summary>
        public int BlankLines
        {
            get { return _blankLines; }
            set { _isValid = true; _blankLines = value; }
        }
    }
}
