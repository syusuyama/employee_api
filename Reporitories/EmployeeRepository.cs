using System.Reflection.Metadata.Ecma335;
using EmployeeModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace EmployeeRepositories

{
    public class EmployeeRepository
    {
        private readonly EmployeeDbContext _context;

        public EmployeeRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task<List<Employee>> GetAllIsValid()
        {
            return await _context.Employees.Where(e => !e.IsDeleted).ToListAsync();
        }

        /// <summary>
        /// 社員情報取得処理
        /// </summary>
        /// <param name="shainNo"></param>
        /// <returns></returns>
        public async Task<Employee?> GetByShainNo(string shainNo)
        {
            return await _context.Employees.SingleOrDefaultAsync(e =>
                                e.ShainNo == shainNo);
                                // && !e.IsDeleted.　画面上からフラグ変更可能とする
        }

        /// <summary>
        /// 登録
        /// </summary>
        /// <returns></returns>
        public async Task Register(string shainNo, CreateEmployeeDto body)
        {
            //登録
            var employee = new EmployeeModel.Employee
            {
                ShainNo = shainNo,
                Name = body.Name,
                Busho = body.Busho,
                Age = body.Age,
                Hobby = body.Hobby,
                IsDeleted = body.IsDeleted
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <returns></returns>
        public async Task UpdateEmployee(Employee employee, UpdateEmployeeDto body)
        {
            //更新
            employee.Name = body.Name;
            employee.Busho = body.Busho;
            employee.Age = body.Age;
            employee.Hobby = body.Hobby;
            employee.IsDeleted = body.IsDeleted;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 削除処理（論理削除）
        /// </summary>
        /// <returns></returns>
        public async Task LogicalDelete(Employee employee)
        {
            //削除（論理削除）
            employee.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 削除処理（物理削除）
        /// </summary>
        /// <returns></returns>
        public async Task PhysicalDelete(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }



    //練習ーーーーーーーーーーーーーーーーー

    public async Task<List<SearchResponceDto>> Search(string? name, int? minAge, int? maxAge, bool sortDesc)
        {
            IQueryable<Employee> employees = _context.Employees.Where(e => !e.IsDeleted);


            if (!string.IsNullOrWhiteSpace(name))
            {
                employees = employees.Where(e => EF.Functions.Like(e.Name.ToLower(), $"%{name.ToLower()}%"));//部分一致
            }

            if (minAge.HasValue)
            {
                employees = employees.Where(e => e.Age >= minAge.Value);
            }

            if (maxAge.HasValue)
            {
                employees = employees.Where(e => e.Age <= maxAge.Value);
            }

            if (sortDesc)
            {
                employees = employees.OrderByDescending(e => e.Age);  
            }
            else
            {
                employees = employees.OrderBy(e => e.Age);  
            }

            var result = employees.Select(g => new SearchResponceDto
                                        {
                                            ShainNo = g.ShainNo,
                                            Name = g.Name,
                                            Busho = g.Busho,
                                            Age = g.Age
                                        }).ToListAsync();

            return await result;
        }



    //練習ーーーーーーーーーーーーーーーーー




    }

}