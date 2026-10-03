using AutoMapper;
using EduTechSpace.Dto;
using EduTechSpace.interfaces;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Interfaces;
using EduTechSpace.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduTechSpace.Services
{
	public class ExamService : IExamService
	{
		private readonly IExamRepository _examRepository;
		private readonly IStudentExamRepository _studentExamRepository;
		private readonly IMapper _mapper;

		public ExamService(
			IExamRepository examRepository,
			IStudentExamRepository studentExamRepository,
			IMapper mapper)
		{
			_examRepository = examRepository;
			_studentExamRepository = studentExamRepository;
			_mapper = mapper;
		}

		public async Task<DataResponseDto<ExamDto>> AddExamAsync(AddExamDto examDto)
		{
			var response = new DataResponseDto<ExamDto>();

			if (examDto != null)
			{
				Exam exam = _mapper.Map<Exam>(examDto);
				await _examRepository.AddExamAsync(exam);
				await _examRepository.SaveChangesAsync();

				response.Success = true;
				response.SuccessMessage = "Exam created successfully.";
				response.Data.Add(_mapper.Map<ExamDto>(exam));
			}
			else
			{
				response.Success = false;
				response.Errors.Add("Empty Exam data was sent.");
			}

			return response;
		}

		public async Task<DataResponseDto<ExamDto>> DeleteExamAsync(int examId)
		{
			var response = new DataResponseDto<ExamDto>();
			var exam = await _examRepository.GetExamByIdAsync(examId);

			if (exam != null)
			{
				await _examRepository.DeleteExamAsync(exam);
				await _examRepository.SaveChangesAsync();

				response.Success = true;
				response.SuccessMessage = "Exam deleted successfully.";
			}
			else
			{
				response.Success = false;
				response.Errors.Add($"Exam with ID {examId} was not found.");
			}

			return response;
		}

		public async Task<DataResponseDto<ExamDto>> DeleteExamWithResultsAsync(int examId)
		{
			var response = new DataResponseDto<ExamDto>();
			var exam = await _examRepository.GetExamByIdAsync(examId);

			if (exam != null)
			{
				// 1. Fetch and delete all related StudentExam results first
				var results = await _studentExamRepository.GetAllExamResultsByExamIdAsync(examId);
				if (results != null && results.Any())
				{
					foreach (var result in results)
					{
						await _studentExamRepository.DeleteExamResultAsync(result);
					}
				}

				// 2. Delete the primary Exam record
				await _examRepository.DeleteExamAsync(exam);
				await _examRepository.SaveChangesAsync();

				response.Success = true;
				response.SuccessMessage = "Exam and all associated results were deleted successfully.";
			}
			else
			{
				response.Success = false;
				response.Errors.Add($"Exam with ID {examId} was not found.");
			}

			return response;
		}

		public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
		{
			var exams = await _examRepository.GetAllExamsAsync();
			return _mapper.Map<IEnumerable<ExamDto>>(exams);
		}

		public async Task<IEnumerable<ExamDto>> GetAllExamsByCourseIdAsync(int courseId)
		{
			var exams = await _examRepository.GetAllExamsByCourseIdAsync(courseId);
			return exams != null ? _mapper.Map<IEnumerable<ExamDto>>(exams) : Enumerable.Empty<ExamDto>();
		}

		public async Task<IEnumerable<ExamDto>> GetAllExamsByInstructorIdAsync(int instructorId)
		{
			var exams = await _examRepository.GetAllExamsByInstructorIdAsync(instructorId);
			return exams != null ? _mapper.Map<IEnumerable<ExamDto>>(exams) : Enumerable.Empty<ExamDto>();
		}

		public async Task<ExamDto?> GetExamByIdAsync(int examId)
		{
			var exam = await _examRepository.GetExamByIdAsync(examId);
			return exam != null ? _mapper.Map<ExamDto>(exam) : null;
		}

		public async Task<DataResponseDto<ExamDto>> UpdateExamAsync(ExamDto examDto)
		{
			var response = new DataResponseDto<ExamDto>();

			if (examDto != null && examDto.Id > 0)
			{
				var existingExam = await _examRepository.GetExamByIdAsync(examDto.Id);

				if (existingExam != null)
				{
					// Use AutoMapper to map the incoming DTO fields onto the existing entity
					_mapper.Map(examDto, existingExam);

					await _examRepository.UpdateExamAsync(existingExam);
					await _examRepository.SaveChangesAsync();

					response.Success = true;
					response.SuccessMessage = "Exam updated successfully.";
					response.Data.Add(_mapper.Map<ExamDto>(existingExam));
				}
				else
				{
					response.Success = false;
					response.Errors.Add($"Exam with ID {examDto.Id} was not found.");
				}
			}
			else
			{
				response.Success = false;
				response.Errors.Add("Invalid Exam data provided for update.");
			}

			return response;
		}
	}
}