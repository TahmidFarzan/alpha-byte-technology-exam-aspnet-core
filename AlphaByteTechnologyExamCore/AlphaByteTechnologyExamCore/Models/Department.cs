﻿using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaByteTechnologyExamCore.Models
{
    [Table("DEPARTMENT")]
    public class Department
    {
        [Key]
        [Column("DEPT_ID")]
        public long Id { get; set; }

        [Column("DIV_ID")]
        public long DivId { get; set; }

        [Column("DEPARTMENT_NAME")]
        public string Name { get; set; }
    }
}