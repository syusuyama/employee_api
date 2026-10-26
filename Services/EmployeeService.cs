using System.Diagnostics;
using EmployeeModel;
using EmployeeRepositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EmployeeServices
{
    public class EmployeeService
    {
        //ファクタリング対象：業務ロジック強化
        private readonly EmployeeRepository _repository;

        public EmployeeService(EmployeeRepository repositories)
        {
            _repository = repositories;
        }

        public Task<List<Employee>> GetAll()
        {
            return _repository.GetAll();
        }

        public Task<List<Employee>> GetAllIsValid()
        {
            return _repository.GetAllIsValid();
        }

        public Task<Employee?> GetByShainNo(string shainNo)
        {
            return _repository.GetByShainNo(shainNo);
        }


        /// <summary>
        /// 社員番号採番処理（あとでDB対応）
        /// 自動採番（社員番号最大値＋１）
        /// 10001番からスタート
        /// 既存の社員がいるにもかかわらず0+1=1になる場合、後続の社員番号重複エラーでハンドリング
        /// </summary>
        public async Task<AssignShainNoResult> AssignShainNo()
        {
            var empList = await _repository.GetAll();
            string tmpShainNo = "";

            if (empList is null || !empList.Any())
            {
                tmpShainNo = "10001"; // 初回登録の場合
            }
            else
            {
                tmpShainNo = (int.Parse(empList.Max(e => e.ShainNo)) + 1).ToString();
                var exists = empList.Any(e => e.ShainNo == tmpShainNo);

                //社員番号重複エラー
                if (exists)
                {
                    return new AssignShainNoResult
                    {
                        Result = EmployeeResult.Conflict,
                        ShainNo = tmpShainNo
                    };
                }
            }

            return new AssignShainNoResult
            {
                Result = EmployeeResult.Success,
                ShainNo = tmpShainNo
            };
        }

        public async Task<AssignShainNoResult> Register(CreateEmployeeDto body)
        {
            var result = await AssignShainNo(); //社員番号採番

            if (result.Result is EmployeeResult.Conflict)
            {
                return new AssignShainNoResult
                    {
                        Result = EmployeeResult.Conflict,
                        ShainNo = result.ShainNo
                    };
            }

            await _repository.Register(result.ShainNo, body);
            return new AssignShainNoResult
                    {
                        Result = EmployeeResult.Success,
                        ShainNo = result.ShainNo
                    };
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeeResult> UpdateEmployee(string shainNo, UpdateEmployeeDto body)
        {
            //更新対象データ存在チェック
            var employee = await _repository.GetByShainNo(shainNo);
            if (employee is null)
            {
                return EmployeeResult.NotFound;
            }

            //更新
            await _repository.UpdateEmployee(employee, body);
            return EmployeeResult.Success;
        }

        /// <summary>
        /// 削除処理（論理削除）
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeeResult> LogicalDelete(string shainNo)
        {
            //削除対象データ存在チェック
            var employee = await _repository.GetByShainNo(shainNo);
            if (employee is null)
            {
                return EmployeeResult.NotFound;
            }

            //削除（論理削除）
            await _repository.LogicalDelete(employee);
            return EmployeeResult.Success;
        }

        /// <summary>
        /// 削除処理（物理削除）
        /// </summary>
        /// <returns></returns>
        public async Task<EmployeeResult> PhysicalDelete(string shainNo)
        {
            //削除対象データ存在チェック
            var employee = await _repository.GetByShainNo(shainNo);
            if (employee is null)
            {
                return EmployeeResult.NotFound;
            }

            //削除（物理削除）
            await _repository.PhysicalDelete(employee);
            return EmployeeResult.Success;
        }


        //練習用ーーーーーーーーーーーーーーーーーーーー

        public async Task<SearchResultDto?> Search(string? name, int? minAge, int? maxAge, bool sortDesc)
        {
            if (minAge.HasValue && maxAge.HasValue &&
                minAge.Value > maxAge.Value)
            {
                return new SearchResultDto
                {
                  result = SearchResult.BadRequest,
                  employees = null
                };
            }

            var empList = await _repository.Search(name, minAge, maxAge, sortDesc);

            return new SearchResultDto
                {
                  result = SearchResult.Success,
                  employees = empList
                };
        }



        //練習用ーーーーーーーーーーーーーーーーーーーー



    };
};
