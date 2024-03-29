﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Post_Service.Data;
using Post_Service.Models.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posting_Service.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public PostController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Post>>> GetAll()
		{
			try
			{
				var result = await context.Post.ToListAsync();
				return Ok(result);
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpGet("GetUserPosts")]
		public async Task<IActionResult> Get(string uid)
		{
			try
			{
				var result = await context.Post.Where(x => x.userId.Equals(uid))
					.OrderBy(x => x.DateTime).ToListAsync();
				return Ok(result);
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Post post)
		{
			try
			{
				await context.Post.AddAsync(post);
				await context.SaveChangesAsync();
				return Created("post", post);
			}
			catch
			{
				return BadRequest("Something went wrong");
			}
		}

		[HttpPut]
		public async Task<IActionResult> UpdateMessage([FromBody] Post post)
		{
			try
			{
				var result = await context.Post.FindAsync(post.Id);
				if (result == null)
				{
					return BadRequest("Post does not exist");
				}
				result.postContent = post.postContent;
				await context.SaveChangesAsync();
				return NoContent();
			}
			catch
			{
				return NotFound();
			}
		}

		[HttpPut("likes")]
		public async Task<IActionResult> UpdateLikes([FromBody] Post post)
		{
			try
			{
				var result = await context.Post.FindAsync(post.Id);
				if (result == null)
				{
					return NotFound();
				}
				result.likes++;
				await context.SaveChangesAsync();
				return NoContent();
			}
			catch
			{
				return BadRequest("Something went wrong");
			}
		}

		// DELETE: api/ApiWithActions/5
		[HttpDelete]
		public async Task<IActionResult> Delete(Post post)
		{
			try
			{
				var result = await context.Post.FindAsync(post.Id);
				if (result == null)
				{
					return NotFound();
				}
				context.Post.Remove(result);
				await context.SaveChangesAsync();
				return StatusCode(202);
			}
			catch
			{
				return BadRequest("Something went wrong");
			}
		}
	}
}