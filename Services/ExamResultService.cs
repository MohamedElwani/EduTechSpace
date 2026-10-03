using AutoMapper;
using EduTechSpace.Dto;
using EduTechSpace.interfaces.ServicesInterfaces;
using EduTechSpace.Interfaces;
using EduTechSpace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduTechSpace.Services
{
	public class ExamResultService : IExamResultService
	{
		private readonly IStudentExamRepository _studentExamRepository;
		private readonly IMapper _mapper;

		public ExamResultService(IStudentExamRepository studentExamRepository, IMapper mapper)
		{
			_mapper = mapper;
			_studentExamRepository = studentExamRepository;
		}

		public async Task<DataResponseDto<ExamResultDto>> AddExamResult(AddExamResultDto examResultDto)
		{
			DataResponseDto<ExamResultDto> dataResponseDto = new DataResponseDto<ExamResultDto>();
			if (examResultDto != null)
			{
				StudentExam studentExam = _mapper.Map<StudentExam>(examResultDto);
				await _studentExamRepository.AddExamResultAsync(studentExam);
				await _studentExamRepository.SaveChangesAsync();

				dataResponseDto.Success = true;
				dataResponseDto.SuccessMessage = "Result was saved";
				// Assuming Data is a List<T> based on your initial Add implementation
				if (dataResponseDto.Data == null) dataResponseDto.Data = new List<ExamResultDto>();

				dataResponseDto.Data.Add(_mapper.Map<ExamResultDto>(studentExam));
			}
			else
			{
				dataResponseDto.Success = false;
				if (dataResponseDto.Errors == null) dataResponseDto.Errors = new List<string>();
				dataResponseDto.Errors.Add("Empty Exam was sent");
			}
			return dataResponseDto;
		}

		public async Task<DataResponseDto<ExamDto>> DeleteExamResult(int examId)
		{
			DataResponseDto<ExamDto> dataResponseDto = new DataResponseDto<ExamDto>();
			if (examId >= 0)
			{
				// Fetching all results by examId to delete them. 
				// (Assuming "examId" implies deleting the results associated with a specific Exam)
				var resultsToDelete = await _studentExamRepository.GetAllExamResultsByExamIdAsync(examId);

				if (resultsToDelete != null && resultsToDelete.Any())
				{
					foreach (var result in resultsToDelete)
					{
						await _studentExamRepository.DeleteExamResultAsync(result);
					}

					await _studentExamRepository.SaveChangesAsync();
					dataResponseDto.Success = true;
					dataResponseDto.SuccessMessage = "Exam results deleted successfully.";
				}
				else
				{
					dataResponseDto.Success = false;
					if (dataResponseDto.Errors == null) dataResponseDto.Errors = new List<string>();
					dataResponseDto.Errors.Add("No results found for the provided Exam ID.");
				}
			}
			else
			{
				dataResponseDto.Success = false;
				if (dataResponseDto.Errors == null) dataResponseDto.Errors = new List<string>();
				dataResponseDto.Errors.Add("Invalid Exam ID.");
			}

			return dataResponseDto;
		}

		public async Task<IEnumerable<ExamWithExamResultsDto>> GetAllExamsWithResultsByCourseIdAsync(int courseId)
		{
			if (courseId < 0) return Enumerable.Empty<ExamWithExamResultsDto>();

			IEnumerable<StudentExam> examResults = await _studentExamRepository.GetAllExamResultsByCourseIdAsync(courseId);

			if (examResults != null && examResults.Any())
			{
				// Group the results by Exam to fit the DTO structure
				var groupedResults = examResults
					.Where(e => e.Exam != null)
					.GroupBy(e => e.Exam)
					.Select(g => new ExamWithExamResultsDto
					{
						Exam = _mapper.Map<ExamDto>(g.Key),
						Results = _mapper.Map<IEnumerable<ExamResultDto>>(g.AsEnumerable())
					});

				return groupedResults.ToList();
			}

			return Enumerable.Empty<ExamWithExamResultsDto>();
		}

		public async Task<IEnumerable<ExamWithExamResultsDto>> GetAllExamsWithResultsByInstructorIdAsync(int instructorId)
		{
			if (instructorId < 0) return Enumerable.Empty<ExamWithExamResultsDto>();

			IEnumerable<StudentExam> examResults = await _studentExamRepository.GetAllExamResultsByInstructorIdAsync(instructorId);

			if (examResults != null && examResults.Any())
			{
				var groupedResults = examResults
					.Where(e => e.Exam != null)
					.GroupBy(e => e.Exam)
					.Select(g => new ExamWithExamResultsDto
					{
						Exam = _mapper.Map<ExamDto>(g.Key),
						Results = _mapper.Map<IEnumerable<ExamResultDto>>(g.AsEnumerable())
					});

				return groupedResults.ToList();
			}

			return Enumerable.Empty<ExamWithExamResultsDto>();
		}

		public async Task<IEnumerable<ExamWithExamResultsDto>> GetAllExamsWithResultsByStudentIdAsync(int studentId)
		{
			if (studentId < 0) return Enumerable.Empty<ExamWithExamResultsDto>();

			IEnumerable<StudentExam> examResults = await _studentExamRepository.GetAllExamResultsByStudentIdAsync(studentId);

			if (examResults != null && examResults.Any())
			{
				var groupedResults = examResults
					.Where(e => e.Exam != null)
					.GroupBy(e => e.Exam)
					.Select(g => new ExamWithExamResultsDto
					{
						Exam = _mapper.Map<ExamDto>(g.Key),
						Results = _mapper.Map<IEnumerable<ExamResultDto>>(g.AsEnumerable())
					});

				return groupedResults.ToList();
			}

			return Enumerable.Empty<ExamWithExamResultsDto>();
		}

		public async Task<DataResponseDto<ExamDto>> UpdateExamResult(ExamResultDto examResultDto)
		{
			DataResponseDto<ExamDto> dataResponseDto = new DataResponseDto<ExamDto>();

			if (examResultDto != null)
			{
				StudentExam studentExam = _mapper.Map<StudentExam>(examResultDto);

				await _studentExamRepository.UpdateExamResultAsync(studentExam);
				await _studentExamRepository.SaveChangesAsync();

				dataResponseDto.Success = true;
				dataResponseDto.SuccessMessage = "Exam result updated successfully.";

				// Assuming you'd want to map the Exam entity back for the Data response
				if (studentExam.Exam != null)
				{
					if (dataResponseDto.Data == null) dataResponseDto.Data = new List<ExamDto>();
					dataResponseDto.Data.Add(_mapper.Map<ExamDto>(studentExam.Exam));
				}
			}
			else
			{
				dataResponseDto.Success = false;
				if (dataResponseDto.Errors == null) dataResponseDto.Errors = new List<string>();
				dataResponseDto.Errors.Add("Empty Exam result payload was sent for update.");
			}

			return dataResponseDto;
		}
	}
}